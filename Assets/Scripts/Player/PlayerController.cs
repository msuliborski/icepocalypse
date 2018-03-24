using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class PlayerController : MonoBehaviour
{

    public float JumpForce;
    public float WallReflectionForce;
    public float MoveSpeed;
    public float _wallTimer = 0; 
    public bool FacingRight = true;
    float _moveDirection = 0;
    
    public bool Grounded = false;
    public bool Walled = false;
    private bool _wallImpact = false; 
    public bool Tubed = false;
    private bool _tubeImpact = false; 
    public bool Stopped = false;
    private bool _stoppedImpact = false; 
    private bool _stoppedReleased = false; 
    public bool Obstacled = false;

    enum PlayerState 
	{
		Idle,
		Running,
		Attacking,
        WallCliming,
        WallHugging,
        ObstacleSliding,
        TubeSliding,
        TubeStopped
	}


    public LayerMask Ground;
    public LayerMask Wall;
    public LayerMask Tube;
    public LayerMask Obstacle;
    public LayerMask Stopper;
    
    private PlayerState _playerState = PlayerState.Idle;

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
        Grounded = Physics2D.IsTouchingLayers(_collider, Ground);
        Walled = Physics2D.IsTouchingLayers(_collider, Wall);
        Tubed = Physics2D.IsTouchingLayers(_collider, Tube);
        Obstacled = Physics2D.IsTouchingLayers(_collider, Obstacle);
        Stopped = Physics2D.IsTouchingLayers(_collider, Stopper);

        if (Grounded && _rigidbody.velocity.x == 0) _playerState = PlayerState.Idle;
        else if (Grounded && _rigidbody.velocity.x != 0) _playerState = PlayerState.Running;
        else if (Tubed) 
        {
            if (Stopped) _playerState = PlayerState.TubeStopped;
            else _playerState = PlayerState.TubeSliding;
        } 
        else if (Walled) _playerState = PlayerState.WallHugging;
        else if (Obstacled) _playerState = PlayerState.ObstacleSliding;

        if (Input.GetKeyDown(KeyCode.RightArrow)) {setFacingRight(true);}
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {setFacingRight(false);} 
        if (Input.GetKeyDown(KeyCode.DownArrow)) _moveDirection = 0;  

/* 
         switch (_playerState)
      {
          case PlayerState.Running:


              break;
          case PlayerState.Running:
              break;
          default:
              break;
      }
*/

        
         if (Grounded)
        {
            _rigidbody.velocity = new Vector2(MoveSpeed * _moveDirection, _rigidbody.velocity.y);   
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce);
            }
        }


        if (Walled)
        {
            _wallTimer += Time.deltaTime;

            if(!_wallImpact)
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
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x - _moveDirection*WallReflectionForce, JumpForce);
               
                if (FacingRight) 
                    setFacingRight(false);
                else 
                    setFacingRight(true);          
            }
        }
        else
        {
            _wallTimer = 0;
            _rigidbody.gravityScale = 10;
            _wallImpact = false;
        }

        if (Tubed)
        { 
            if(!_tubeImpact)
            {
                
                _tubeImpact = true;
                _rigidbody.velocity = new Vector2(0, 0);
                _rigidbody.gravityScale = 1f;
                
            }

            if (_rigidbody.velocity.y <= -3 && !Stopped) _rigidbody.velocity = new Vector2(0, -3);
            
            

            if (Stopped)
            {
                if(!_stoppedImpact)
                {
                    _rigidbody.velocity = new Vector2(0, 0);
                    _stoppedImpact = true;
                }
                
                if (!_stoppedReleased)_rigidbody.gravityScale = 0;

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    _rigidbody.gravityScale = 10;
                    _stoppedReleased = true;
                }

            }
            else 
            {
                if (!_stoppedReleased) _rigidbody.gravityScale = 1;
                else _rigidbody.gravityScale = 10;
                _stoppedImpact = false;
            }
            
        }
        else 
        {
            _tubeImpact = false;
            if (!Walled)_rigidbody.gravityScale = 10;
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


