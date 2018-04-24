using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;


public class PlayerControllerExperimental : MonoBehaviour
{
    public UnityEvent TubeStopperDestroy;
    public float JumpForce = 18;
    public float WallReflectionForce = 17;
    public float MoveSpeed = 10;
    public float _wallTimer = 0;
    public bool FacingRight = true;
    private float _moveDirection = 0;

    private bool _wallImpact = false;
    private bool _tubeImpact = false;
    private bool _stoppedImpact = false;
    private bool _slopeImpact = false;
    public bool _obstacleHasJumped = false;
    private bool _tubeIgnore = false;
    
    private List<Vector3> _scriptDestinations;
    private float _scriptSpeed;
    public bool _isScripting = false;
    public int _index;
    public int _indexToReact = -1;    

    public enum PlayerState
    {
        Inert,
        Grounded,
        Attacking,
        WallClimbing,
        HandBarring,
        WallHugging,
        EgdeClimbingBody,
        EgdeClimbingCorner,
        TubeSliding,
        TubeStopped,
        Sloping,
    }

	public PlayerState CurrentState = PlayerState.Inert;

    private LayerMask Ground;
    private LayerMask Wall;
    private LayerMask Tube;
    private LayerMask Obstacle_R;
    private LayerMask Obstacle_L;
    private LayerMask HandBar;
    private LayerMask Stopper;
    private LayerMask Slope;
    private LayerMask Edge;

    private Rigidbody2D _rigidbody;
    private Collider2D _colliderWhole;
    private Collider2D _colliderBody;
    private Collider2D _colliderLegs;
    private Collider2D _colliderCorner;

    private float playerWidth;
    private float playerHeight;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isScripting)
        {
            if (collision.gameObject.transform.name == "Wall")
            {
                //_index = 0;
                _scriptDestinations.Clear();
                _scriptDestinations.Add(collision.gameObject.transform.position + new Vector3(-1f , collision.gameObject.transform.localScale.y/2 - 1f, 0));
                _scriptDestinations.Add(_scriptDestinations[0] + new Vector3(0, playerHeight, 0));
                _scriptDestinations.Add(_scriptDestinations[1] + new Vector3(1f, 0, 0));
            }
        }
    }

    private void script()
    {
        transform.position = Vector2.MoveTowards(_rigidbody.transform.position, _scriptDestinations[_index], _scriptSpeed * Time.deltaTime);
        if (_rigidbody.transform.position == _scriptDestinations[_index])
        {
            if (_index < _scriptDestinations.Count - 1)
            {
                if (_indexToReact == _index)
                {
                    if (Input.GetKeyDown(KeyCode.Space)) OnKeySpace();
                }
                else _index++;
            }
            else
            {
                _rigidbody.isKinematic = false;
                _isScripting = false;
            }
        }
    }
    
    void Start()
     {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _colliderBody = GameObject.FindGameObjectWithTag("body_collider").GetComponent<Collider2D>();
        _colliderCorner = GameObject.FindGameObjectWithTag("corner_collider").GetComponent<Collider2D>();
        _colliderLegs = GameObject.FindGameObjectWithTag("legs_collider").GetComponent<Collider2D>();
        _colliderWhole = gameObject.GetComponent<Collider2D>(); 
        Ground = LayerMask.GetMask("Ground");
        Wall = LayerMask.GetMask("Wall");
        Tube = LayerMask.GetMask("Tube");
        Stopper = LayerMask.GetMask("TubeStopper");
        HandBar = LayerMask.GetMask("Handbar");
        Slope = LayerMask.GetMask("Slope");
        Edge = LayerMask.GetMask("Edge");
        playerWidth = _rigidbody.transform.localScale.x;
        playerHeight = _rigidbody.transform.localScale.y;
        _scriptDestinations = new List<Vector3>();
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.P)) Debug.Break();

        if (_isScripting) script();
        else
        {
            if (Physics2D.IsTouchingLayers(_colliderLegs, Ground)) CurrentState = PlayerState.Grounded;
            else if (Physics2D.IsTouchingLayers(_colliderBody, Tube) && !_tubeIgnore)
            {
                if (Physics2D.IsTouchingLayers(_colliderLegs, Stopper)) CurrentState = PlayerState.TubeStopped;
                else CurrentState = PlayerState.TubeSliding;
            }
            else if (Physics2D.IsTouchingLayers(_colliderBody, Wall)) CurrentState = PlayerState.WallHugging;
            else if (Physics2D.IsTouchingLayers(_colliderBody, Edge)) CurrentState = PlayerState.EgdeClimbingBody;
            else if (Physics2D.IsTouchingLayers(_colliderCorner, Edge)) CurrentState = PlayerState.EgdeClimbingCorner; 
            else if (Physics2D.IsTouchingLayers(_colliderBody, Wall)) CurrentState = PlayerState.WallHugging;
            else if (Physics2D.IsTouchingLayers(_colliderWhole, HandBar)) CurrentState = PlayerState.HandBarring;
            else if (Physics2D.IsTouchingLayers(_colliderWhole, Slope)) CurrentState = PlayerState.Sloping;
            else CurrentState = PlayerState.Inert;

            switch (CurrentState)
            {
                case PlayerState.Grounded:

                    movement();

                    if (Input.GetKeyDown(KeyCode.RightArrow)) SetFacingRight(true);
                    if (Input.GetKeyDown(KeyCode.LeftArrow)) SetFacingRight(false);
                    if (Input.GetKeyDown(KeyCode.DownArrow)) OnKeyDown();
                    if (Input.GetKeyDown(KeyCode.Space)) OnKeySpace();
                    resetImpacts();
                    _tubeIgnore = false;

                    break;

                case PlayerState.Inert:

                    resetImpacts();

                    break;

                case PlayerState.WallHugging:

                    onWallImpact();
                    manageWallTimer();

                    if ((Input.GetKeyDown(KeyCode.Space))) OnKeySpace();

                    break;

                case PlayerState.Sloping:

                    onSlopeImpact();

                    transform.rotation = Quaternion.Euler(0, 0, -30);

                    if ((Input.GetKeyDown(KeyCode.Space))) OnKeySpace();

                    break;

                case PlayerState.TubeSliding:

                    onTubeImpact();

                    if (_rigidbody.velocity.y <= -3) _rigidbody.velocity = new Vector2(0, -3);

                    break;

                case PlayerState.TubeStopped:

                    onTubeStopperImpact();

                    if (Input.GetKeyDown(KeyCode.DownArrow)) OnKeyDown();

                    break;

                case PlayerState.EgdeClimbingBody:

                    onEdgeBodyImpact(4.0f);

                    break;

                case PlayerState.EgdeClimbingCorner:

                    onEdgeCornerImpact(4.0f);

                    break;

                
            }
        }
    }

    public void OnKeyDown()
    {
        switch (CurrentState)
        {
            case PlayerState.TubeStopped:
                _rigidbody.gravityScale = 10;
                TubeStopperDestroy.Invoke();
                _tubeIgnore = true;
                break;
            
            default:
                StopMovement();
                break;
        }
    }
    private void movement()
    {
        _rigidbody.velocity = new Vector2(MoveSpeed * _moveDirection, _rigidbody.velocity.y);
    }

    public void StopMovement()
    {
        _moveDirection = 0;
    }

    public void OnKeySpace()
    {
        if (_isScripting) _index++;
        else
        {
            if (CurrentState == PlayerState.Grounded || CurrentState == PlayerState.Sloping) _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce);
            else if (CurrentState == PlayerState.WallHugging)
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x - _moveDirection * WallReflectionForce, JumpForce);

                if (FacingRight)
                    SetFacingRight(false);
                else
                    SetFacingRight(true);
            }
        }
    }

    public void SetFacingRight(bool _facingRight)
    {
        if (_facingRight)
        {
            FacingRight = true;
            if (transform.localScale.x < 0) transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            _moveDirection = 1;
        }
        else
        {
            FacingRight = false;
            if (transform.localScale.x > 0) transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            _moveDirection = -1;
        }
    }

    private void manageWallTimer()
    {
        _wallTimer += Time.deltaTime;
        if (_wallTimer >= 0.15)
        {
            _rigidbody.gravityScale = 3;
            if (_rigidbody.velocity.y <= -4) _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, -4);
        }
    }

    private void onSlopeImpact()
    {
        if (!_slopeImpact)
        {
            _slopeImpact = true;
            SetFacingRight(true);
            _rigidbody.gravityScale = 2;
        }
    }

    private void onTubeStopperImpact()
    {
        if (!_stoppedImpact)
        {
            _rigidbody.velocity = new Vector2(0, 0);
            _stoppedImpact = true;
            _rigidbody.gravityScale = 0;
        }
    }

    private void onTubeImpact()
    {
        if (!_tubeImpact)
        {
            _tubeImpact = true;
            _rigidbody.velocity = new Vector2(0, 0);
            _rigidbody.gravityScale = 1f;
        }
    }

    void onEdgeCornerImpact(float speed)
    {
        _scriptSpeed = speed;
        _isScripting = true;
        _rigidbody.isKinematic = true;
        _rigidbody.velocity = new Vector3(0, 0, 0);
        _index = 0;
        _indexToReact = 0;
    }

    void onEdgeBodyImpact(float speed)
    {
        _scriptSpeed = speed;
        _isScripting = true;
        _rigidbody.isKinematic = true;
        _rigidbody.velocity = new Vector3(0, 0, 0);
        _index = 1;
        _indexToReact = -1;
    }

    private void onWallImpact()
    {
        if (!_wallImpact)
        {
            _wallImpact = true;
            _rigidbody.velocity = new Vector2(0, 0);
            _rigidbody.gravityScale = 0;
        }

    }

    private void resetImpacts()
    {
        _wallTimer = 0;
        _rigidbody.gravityScale = 10;
        _wallImpact = false;
        _tubeImpact = false;
        _stoppedImpact = false;
        _slopeImpact = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        _obstacleHasJumped = false;
    }
}



/*
          case PlayerState.ObstacleClimbing_L:

               if (!_obstacleHasJumped)
               {
                   if (Input.GetKey(KeyCode.Space)) Jump();
                   else if(Input.GetKeyDown(KeyCode.LeftArrow))
                   {
                       SetFacingRight(false);
                       transform.position = new Vector2(transform.position.x - 0.3f, transform.position.y);
                   }
               }
               else _rigidbody.velocity = new Vector2(8, 5);

               break;

           case PlayerState.ObstacleClimbing_R:

               if (!_obstacleHasJumped)
               {
                   if (Input.GetKeyDown(KeyCode.Space)) Jump();
                   else if(Input.GetKeyDown(KeyCode.RightArrow))
                   {
                       SetFacingRight(false);
                       transform.position = new Vector2(transform.position.x + 0.3f, transform.position.y);
                   }
               }
               else _rigidbody.velocity = new Vector2(-8, 5);

               break;
           */
