using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;


public class PlayerControllerExperimental : MonoBehaviour
{
    public UnityEvent TubeStopperDestroy;
    public float JumpForce;
    public float WallReflectionForce;
    public float MoveSpeed;
    public float _wallTimer = 0;
    public bool FacingRight = true;
    float _moveDirection = 0;

   
    private bool _wallImpact = false;
    private bool _tubeImpact = false;
    private bool _stoppedImpact = false;
    private bool _stoppedReleased = false;
   

    public enum PlayerState
    {
        Inert,
        Grounded,
        Attacking,
        WallClimbing,
        WallHugging,
        ObstacleSliding,
        TubeSliding,
        TubeStopped
    }

    public PlayerState CurrentState = PlayerState.Inert;

    public LayerMask Ground;
    public LayerMask Wall;
    public LayerMask Tube;
    public LayerMask Obstacle;
    public LayerMask Stopper;

    

    private Rigidbody2D _rigidbody;
    private Collider2D _collider;



    // Use this for initialization
    void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _collider = gameObject.GetComponent<Collider2D>();
    }


    void Update()
    {
        if (Physics2D.IsTouchingLayers(_collider, Ground)) CurrentState = PlayerState.Grounded;
        else if (Physics2D.IsTouchingLayers(_collider, Wall)) CurrentState = PlayerState.WallHugging;
        else if (Physics2D.IsTouchingLayers(_collider, Obstacle)) CurrentState = PlayerState.ObstacleSliding;
        else if (Physics2D.IsTouchingLayers(_collider, Tube))
        {
            if (Physics2D.IsTouchingLayers(_collider, Stopper)) CurrentState = PlayerState.TubeStopped;
            else CurrentState = PlayerState.TubeSliding;
        }
        else CurrentState = PlayerState.Inert;
        

         
        switch (CurrentState)
        {
            case PlayerState.WallHugging:

                _wallTimer += Time.deltaTime;

                if (!_wallImpact)
                {
                    _wallImpact = true;
                    _rigidbody.velocity = new Vector2(0, 0);
                    _rigidbody.gravityScale = 0;
                }

                if (_wallTimer >= 0.15)
                {
                    _rigidbody.gravityScale = 3;
                    if (_rigidbody.velocity.y <= -4) _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, -4);
                }

                if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
                {
                    _rigidbody.velocity = new Vector2(_rigidbody.velocity.x - _moveDirection * WallReflectionForce, JumpForce);

                    if (FacingRight)
                        setFacingRight(false);
                    else
                        setFacingRight(true);
                }

                break;

            case PlayerState.Grounded:

                _rigidbody.velocity = new Vector2(MoveSpeed * _moveDirection, _rigidbody.velocity.y);
                if (Input.GetKeyDown(KeyCode.RightArrow)) setFacingRight(true); 
                if (Input.GetKeyDown(KeyCode.LeftArrow)) setFacingRight(false); 
                if (Input.GetKeyDown(KeyCode.DownArrow)) _moveDirection = 0;
                if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))) _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce);

                _wallTimer = 0;
                _rigidbody.gravityScale = 10;
                _wallImpact = false;
                _tubeImpact = false;
                _stoppedImpact = false;
                _stoppedReleased = false;

                break;

            case PlayerState.TubeSliding:

                if (!_tubeImpact)
                {

                    _tubeImpact = true;
                    _rigidbody.velocity = new Vector2(0, 0);
                    _rigidbody.gravityScale = 1f;

                }

                if (_rigidbody.velocity.y <= -3) _rigidbody.velocity = new Vector2(0, -3);


                _wallImpact = false;

                break;

            case PlayerState.TubeStopped:

                
                if (!_stoppedImpact)
                {
                    _rigidbody.velocity = new Vector2(0, 0);
                    _stoppedImpact = true;
                    _rigidbody.gravityScale = 0;
                }

                //if (!_stoppedReleased) _rigidbody.gravityScale = 0;

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    _rigidbody.gravityScale = 10;
                    TubeStopperDestroy.Invoke();
                    Debug.Log("Chuj");
                    //_stoppedReleased = true;
                    
                }


                break;


            case PlayerState.Inert:

                if (Input.GetKeyDown(KeyCode.RightArrow)) setFacingRight(true);
                if (Input.GetKeyDown(KeyCode.LeftArrow)) setFacingRight(false);
                _wallTimer = 0;
                _rigidbody.gravityScale = 10;
                _wallImpact = false;
                _tubeImpact = false;
                _stoppedImpact = false;
                _stoppedReleased = false;

                break;
        }
        


       

       
        


    }

    void setFacingRight(bool _facingRight)
    {
        if (_facingRight)
        {
            FacingRight = true;
            if (transform.localScale.x > 0) transform.localScale *= 1;
            else transform.localScale *= -1;
            _moveDirection = 1;
        }
        else
        {
            FacingRight = false;
            if (transform.localScale.x > 0) transform.localScale *= -1;
            else transform.localScale *= 1;
            _moveDirection = -1;
        }


    }
}

/*
        Grounded = Physics2D.IsTouchingLayers(_collider, Ground);
        Walled = Physics2D.IsTouchingLayers(_collider, Wall);

        if (Grounded)
        {
            _rigidbody.velocity = new Vector2(MoveSpeed * _moveDirection, _rigidbody.velocity.y);           
            
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce);
            }
        }

        if (Walled && _rigidbody.velocity.y < 0)
        {
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            {
                          
                if (Facing == Sides.RIGHT)
                {
                     _rigidbody.velocity = new Vector2(_rigidbody.velocity.x - BounceFromWallForce, JumpForce);
                    Facing = Sides.LEFT;
                    Flip();
                }
                else 
                {
                     _rigidbody.velocity = new Vector2(_rigidbody.velocity.x + BounceFromWallForce, JumpForce);
                    Facing = Sides.RIGHT;
                    Flip();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) _moveDirection++;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) _moveDirection--;
        if (Input.GetKeyUp(KeyCode.RightArrow)) _moveDirection--;
        if (Input.GetKeyUp(KeyCode.LeftArrow)) _moveDirection++;
        
        if (_moveDirection == 1)
        {
            if (Facing == Sides.RIGHT)
            {
                Facing = Sides.LEFT;
                Flip();
            }
        }
        else if (_moveDirection == -1)
        {
            if (Facing == Sides.LEFT)
            {
                Facing = Sides.RIGHT;
                Flip();
            }
        }
    }
    void Flip()
    {
        Vector2 localScale = gameObject.transform.localScale;
        localScale.x *= -1;
        gameObject.transform.localScale = localScale;
    }

     */


