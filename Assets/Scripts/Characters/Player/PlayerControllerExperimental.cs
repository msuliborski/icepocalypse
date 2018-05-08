using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;


public class PlayerControllerExperimental : MonoBehaviour
{
    #region Variables

    public UnityEvent TubeStopperDestroy;
    public float JumpForce = 18;
    public float WallReflectionForce = 17;
    public float MoveSpeed = 10;
    public int Hp = 100;
    private float _wallTimer = 0;
    public bool FacingRight = true;
    private float _moveDirection = 0;

    private bool _wallImpact = false;
    private bool _tubeImpact = false;
    private bool _stoppedImpact = false;
    private bool _slopeImpact = false;
    
    
    private List<Vector3> _scriptDestinations;
    private float _scriptSpeed;
    private bool _isScripting = false;
    private int _index;
    private int _indexToReact = -1;
    private KeyCode _keyToReact;

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
        Sloping,
        Death
    }

	public PlayerState CurrentState = PlayerState.Inert;

    private LayerMask Ground;
    private LayerMask Wall;
    private LayerMask Tube;
    private LayerMask Obstacle_R;
    private LayerMask Obstacle_L;
    private LayerMask HandBar;
    private LayerMask Slope;
    private LayerMask Edge;

    private Rigidbody2D _rigidbody;
    private Collider2D _colliderWhole;
    private Collider2D _colliderBody;
    private Collider2D _colliderLegs;
    private Collider2D _colliderCorner;

    private float tubeHeight;
    private float playerWidth;
    private float playerHeight;

    #endregion

    #region Start&Update
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
            if (Hp > 0)
            {
                if (Physics2D.IsTouchingLayers(_colliderBody, Edge)) CurrentState = PlayerState.EgdeClimbingBody;
                else if (Physics2D.IsTouchingLayers(_colliderCorner, Edge)) CurrentState = PlayerState.EgdeClimbingCorner;
                else if (Physics2D.IsTouchingLayers(_colliderLegs, Ground)) CurrentState = PlayerState.Grounded;
                else if (Physics2D.IsTouchingLayers(_colliderWhole, Tube)) CurrentState = PlayerState.TubeSliding;
                else if (Physics2D.IsTouchingLayers(_colliderBody, Wall)) CurrentState = PlayerState.WallHugging;
                else if (Physics2D.IsTouchingLayers(_colliderBody, Wall)) CurrentState = PlayerState.WallHugging;
                else if (Physics2D.IsTouchingLayers(_colliderWhole, HandBar)) CurrentState = PlayerState.HandBarring;
                else if (Physics2D.IsTouchingLayers(_colliderWhole, Slope)) CurrentState = PlayerState.Sloping;
                else CurrentState = PlayerState.Inert;
            }
            else CurrentState = PlayerState.Death;

            switch (CurrentState)
            {
                case PlayerState.Grounded:

                    movement();

                    if (Input.GetKeyDown(KeyCode.RightArrow)) SetFacingRight(true);
                    if (Input.GetKeyDown(KeyCode.LeftArrow)) SetFacingRight(false);
                    if (Input.GetKeyDown(KeyCode.DownArrow)) OnKeyDown();
                    if (Input.GetKeyDown(KeyCode.Space)) OnKeySpace();
                    resetImpacts();

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

                    break;

               case PlayerState.EgdeClimbingBody:

                    onEdgeBodyImpact();

                    break;

                case PlayerState.EgdeClimbingCorner:

                    onEdgeCornerImpact();

                    break;

                case PlayerState.Death:

                    // TO DO STH WITH GAME & DEATH ANIMATION

                    break;
            }
        }
    }
    #endregion

    #region Input
    public void OnKeyDown()
    {
        if (_isScripting) _index++;
        else
        {
            switch (CurrentState)
            {
               default:
                    StopMovement();
                    break;
            }
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
    #endregion

    #region Impacts
    private void onSlopeImpact()
    {
        if (!_slopeImpact)
        {
            _slopeImpact = true;
            SetFacingRight(true);
            _rigidbody.gravityScale = 2;
        }
    }
    private void onTubeImpact()
    {
        _scriptDestinations.Clear();
        GameObject go = findClosestObjectWithTag("WallTube", 1);
        tubeHeight = go.GetComponent<SpriteRenderer>().bounds.size.y;
        _scriptDestinations.Add(go.transform.position + new Vector3(playerWidth, tubeHeight / 2 + playerHeight / 2, 0f));
        _scriptDestinations.Add(_scriptDestinations[0] + new Vector3(0f, -tubeHeight, 0f));
        _scriptSpeed = 3.0f;
        _isScripting = true;
        _rigidbody.isKinematic = true;
        _rigidbody.velocity = new Vector3(0, 0, 0);
        _index = 0;
        _indexToReact = 1;
        _keyToReact = KeyCode.DownArrow;
    }

    void onEdgeCornerImpact()
    {
        _scriptDestinations.Clear();
        GameObject go = findClosestObjectWithTag("Wall", 1);
        if (FacingRight)
        {
            _scriptDestinations.Add(go.transform.position + new Vector3(-1f, go.transform.localScale.y / 2 - 1f, 0f));
            _scriptDestinations.Add(_scriptDestinations[0] + new Vector3(0, playerHeight, 0));
            _scriptDestinations.Add(_scriptDestinations[1] + new Vector3(1f, 0, 0));
        }
        else
        {
            _scriptDestinations.Add(go.transform.position + new Vector3(1f, go.transform.localScale.y / 2 - 1f, 0f));
            _scriptDestinations.Add(_scriptDestinations[0] + new Vector3(0, playerHeight, 0));
            _scriptDestinations.Add(_scriptDestinations[1] + new Vector3(-1f, 0, 0));
        }
        _scriptSpeed = 4.0f;
        _isScripting = true;
        _rigidbody.isKinematic = true;
        _rigidbody.velocity = new Vector3(0, 0, 0);
        _index = 0;
        _indexToReact = 0;
        _keyToReact = KeyCode.Space;
    }

    void onEdgeBodyImpact()
    {
         _scriptDestinations.Clear();
        GameObject go = findClosestObjectWithTag("Wall", 1);
        if (FacingRight)
        {
            _scriptDestinations.Add(go.transform.position + new Vector3(-1f, go.transform.localScale.y / 2 + playerHeight / 2, 0f));
            _scriptDestinations.Add(_scriptDestinations[0] + new Vector3(1f, 0, 0));
        }
        else
        {
            _scriptDestinations.Add(go.transform.position + new Vector3(1f, go.transform.localScale.y / 2 + playerHeight / 2, 0f));
            _scriptDestinations.Add(_scriptDestinations[0] + new Vector3(-1f, 0, 0));
        }
        _scriptSpeed = 4.0f;
        _isScripting = true;
        _rigidbody.isKinematic = true;
        _rigidbody.velocity = new Vector3(0, 0, 0);
        _index = 0;
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
    }
    #endregion

    #region AdditionalFuncs
    private void manageWallTimer()
    {
        _wallTimer += Time.deltaTime;
        if (_wallTimer >= 0.15)
        {
            _rigidbody.gravityScale = 3;
            if (_rigidbody.velocity.y <= -4) _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, -4);
        }
    }


    private void script()
    {
        if (_index > _scriptDestinations.Count - 1)
        {
            _rigidbody.isKinematic = false;
            _isScripting = false;
            return;
        }
        transform.position = Vector2.MoveTowards(_rigidbody.transform.position, _scriptDestinations[_index], _scriptSpeed * Time.deltaTime);
        if (_rigidbody.transform.position == _scriptDestinations[_index])
        {
            if (_indexToReact == _index)
            {
                if (Input.GetKeyDown(_keyToReact))
                {
                    switch (_keyToReact)
                    {
                        case KeyCode.Space:
                            OnKeySpace();
                            break;
                        case KeyCode.DownArrow:
                            OnKeyDown();
                            break;
                    }
                }
            }
            else _index++;
        }
    }

    private GameObject findClosestObjectWithTag(string tag, int side)
    {
        // side: 1 - top, 2 - left, 3 - right, 4 - bottom, 5 - center
        List<GameObject> gos = new List<GameObject>(GameObject.FindGameObjectsWithTag(tag));
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = _colliderCorner.transform.position;
        Vector2 diff;
        foreach (GameObject go in gos)
        {
            switch (side)
            {
                case 1:
                    diff = (go.transform.position + new Vector3(0f, go.GetComponent<SpriteRenderer>().bounds.size.y)) - position;
                    break;
                default:
                    diff = go.transform.position - position;
                    break;
            }
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }
    #endregion
}



