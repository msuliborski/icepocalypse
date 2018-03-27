﻿using System.Collections;
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
    public bool _obstacleHasJumped = false;
    private bool _tubeIgnore = false;


    public enum PlayerState
    {
        Inert,
        Grounded,
        Attacking,
        WallClimbing,
        HandBarring,
        WallHugging,
        ObstacleClimbing_L,
        ObstacleClimbing_R,
        TubeSliding,
        TubeStopped,
        Sloping
    }

    public PlayerState CurrentState = PlayerState.Inert;

    public LayerMask Ground;
    public LayerMask Wall;
    public LayerMask Tube;
    public LayerMask Obstacle_R;
    public LayerMask Obstacle_L;
    public LayerMask HandBar;
    public LayerMask Stopper;
    public LayerMask Slope;



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
        if (Physics2D.IsTouchingLayers(_collider, Obstacle_R)) CurrentState = PlayerState.ObstacleClimbing_R;
        else if (Physics2D.IsTouchingLayers(_collider, Obstacle_L)) CurrentState = PlayerState.ObstacleClimbing_L;
        else if (Physics2D.IsTouchingLayers(_collider, Tube) && !_tubeIgnore)
        {
            if (Physics2D.IsTouchingLayers(_collider, Stopper)) CurrentState = PlayerState.TubeStopped;
            else CurrentState = PlayerState.TubeSliding;
        }
        
        else if (Physics2D.IsTouchingLayers(_collider, Ground)) CurrentState = PlayerState.Grounded;
        else if (Physics2D.IsTouchingLayers(_collider, Wall)) CurrentState = PlayerState.WallHugging;
        else if (Physics2D.IsTouchingLayers(_collider, HandBar)) CurrentState = PlayerState.HandBarring;
        else if (Physics2D.IsTouchingLayers(_collider, Slope)) CurrentState = PlayerState.Sloping;
        else CurrentState = PlayerState.Inert;




        switch (CurrentState)
        {


            case PlayerState.Grounded:
                //Debug.Log("setting velocity " + Time.deltaTime);
                //Debug.Log("move direction " + _moveDirection);
                _rigidbody.velocity = new Vector2(MoveSpeed * _moveDirection, _rigidbody.velocity.y);
                if (Input.GetKeyDown(KeyCode.RightArrow)) SetFacingRight(true);
                if (Input.GetKeyDown(KeyCode.LeftArrow)) SetFacingRight(false);
                if (Input.GetKeyDown(KeyCode.DownArrow)) OnKeyDown();
                if (Input.GetKeyDown(KeyCode.Space)) Jump();

                resetImpacts();
                _tubeIgnore = false;

                break;


            case PlayerState.Inert:

                if (Input.GetKeyDown(KeyCode.RightArrow)) SetFacingRight(true);
                if (Input.GetKeyDown(KeyCode.LeftArrow)) SetFacingRight(false);
                resetImpacts();

                break;

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

                if ((Input.GetKeyDown(KeyCode.Space))) Jump();
                
                  
                break;

            case PlayerState.Sloping:


                transform.rotation = Quaternion.Euler(0, 0, -30);
                SetFacingRight(true);
                _rigidbody.gravityScale = 2;
                if ((Input.GetKeyDown(KeyCode.Space))) Jump();

                break;

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
                else
                {
                    _rigidbody.velocity = new Vector2(7, 7);

                }
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
                else
                {
                    _rigidbody.velocity = new Vector2(-7, 7);

                }
                break;

            case PlayerState.TubeSliding:

                if (!_tubeImpact)
                {

                    _tubeImpact = true;
                    _rigidbody.velocity = new Vector2(0, 0);
                    _rigidbody.gravityScale = 1f;

                }

                if (_rigidbody.velocity.y <= -3) _rigidbody.velocity = new Vector2(0, -3);

                break;

            case PlayerState.TubeStopped:


                if (!_stoppedImpact)
                {
                    _rigidbody.velocity = new Vector2(0, 0);
                    _stoppedImpact = true;
                    _rigidbody.gravityScale = 0;
                }



                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    OnKeyDown();
                }


                break;




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


    public void StopMovement()
    {
        _moveDirection = 0;
    }

    public void Jump()
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
         else if (CurrentState == PlayerState.ObstacleClimbing_R || CurrentState == PlayerState.ObstacleClimbing_L) _obstacleHasJumped = true;




    }

    public void SetFacingRight(bool _facingRight)
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
        
        Debug.Log("wystawiamy asd");

    }


    private void resetImpacts()
    {
        _wallTimer = 0;
        _rigidbody.gravityScale = 10;
        _wallImpact = false;
        _tubeImpact = false;
        _stoppedImpact = false;
        _obstacleHasJumped = false;
        _rigidbody.isKinematic = false;
        _obstacleHasJumped = false;
      //  _tubeIgnore = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);



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


