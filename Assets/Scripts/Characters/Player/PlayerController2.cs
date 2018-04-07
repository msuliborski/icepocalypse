using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;


public class PlayerController2 : MonoBehaviour
{
    public UnityEvent TubeStopperDestroy;
    public float JumpForce;
    public float WallReflectionForce;
    public float MoveSpeed;
    public float _wallTimer = 0;
    public bool FacingRight = true;
    float _moveDirection = 0;

    public enum State
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
        Sloping,
        Cornering
    }


    public State CurrentState = State.Inert;
    
   // public bool _grounded = false;
   // public bool _walled = false;
    //public bool _inert = false;


    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    public LayerMask Ground;


    // Use this for initialization
    void Start()
    {
       // Ground = LayerMask.GetMask("Ground");
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _collider = gameObject.GetComponent<Collider2D>();
    }


    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            if (CurrentState != State.Grounded)
            {
                _rigidbody.velocity = new Vector2(0, 0);
                _rigidbody.gravityScale = 0;
            }
            
        }

    }

  /*  void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.layer == LayerMask.GetMask("Wall"))
        {
            if (CurrentState == State.Grounded)
            {
                CurrentState = State.WallHugging;
                _rigidbody.velocity = new Vector2(0, 0);
                _rigidbody.gravityScale = 0;
            }


        }
        
    }*/

    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (CurrentState == State.Cornering)
            {
               _rigidbody.velocity = new Vector2(0, 0);
                _rigidbody.gravityScale = 0;
            }
        }
        else if (coll.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            if (CurrentState == State.WallHugging)
            {
                _rigidbody.gravityScale = 10;
                _wallTimer = 0;
            }
        }
       
    }


    void LateUpdate()
    {

        if (Physics2D.IsTouchingLayers(_collider, LayerMask.GetMask("Ground")))
        {
            if (Physics2D.IsTouchingLayers(_collider, LayerMask.GetMask("Wall"))) CurrentState = State.Cornering;
            else CurrentState = State.Grounded;
        }
       else if (Physics2D.IsTouchingLayers(_collider, LayerMask.GetMask("Tube")))
        {
            if (Physics2D.IsTouchingLayers(_collider, LayerMask.GetMask("Stopper"))) CurrentState = State.TubeStopped;
            else CurrentState = State.TubeSliding;
        }
        else if (Physics2D.IsTouchingLayers(_collider, LayerMask.GetMask("Wall"))) CurrentState = State.WallHugging;
        else if (Physics2D.IsTouchingLayers(_collider, LayerMask.GetMask("Handbar"))) CurrentState = State.HandBarring;
        else if (Physics2D.IsTouchingLayers(_collider, LayerMask.GetMask("Slope"))) CurrentState = State.Sloping;
        else CurrentState = State.Inert;



        switch (CurrentState)
        {

            case State.Grounded:
                _rigidbody.velocity = new Vector2(MoveSpeed * _moveDirection, _rigidbody.velocity.y);
                if (Input.GetKeyDown(KeyCode.RightArrow)) SetFacingRight(true);
                if (Input.GetKeyDown(KeyCode.LeftArrow)) SetFacingRight(false);
                if (Input.GetKeyDown(KeyCode.DownArrow)) OnKeyDown();
                if (Input.GetKeyDown(KeyCode.Space)) Jump();
                break;

            case State.WallHugging:
                _wallTimer += Time.deltaTime;
                manageWallTimer();
                if ((Input.GetKeyDown(KeyCode.Space))) Jump();
                break;

            case State.Cornering:
                _rigidbody.velocity = new Vector2(MoveSpeed * _moveDirection, _rigidbody.velocity.y);
                if (Input.GetKeyDown(KeyCode.RightArrow)) SetFacingRight(true);
                if (Input.GetKeyDown(KeyCode.LeftArrow)) SetFacingRight(false);
                if (Input.GetKeyDown(KeyCode.DownArrow)) OnKeyDown();
                if (Input.GetKeyDown(KeyCode.Space)) Jump();
                break;
                
        }

        /*
        if (_grounded)
        {
            _rigidbody.velocity = new Vector2(MoveSpeed * _moveDirection, _rigidbody.velocity.y);
            if (Input.GetKeyDown(KeyCode.RightArrow)) SetFacingRight(true);
            if (Input.GetKeyDown(KeyCode.LeftArrow)) SetFacingRight(false);
            if (Input.GetKeyDown(KeyCode.DownArrow)) OnKeyDown();
            if (Input.GetKeyDown(KeyCode.Space)) Jump();

        }
        else if (_walled)
        {
            _wallTimer += Time.deltaTime;

            manageWallTimer();

            if ((Input.GetKeyDown(KeyCode.Space))) Jump();

        }
        */





    }

    public void OnKeyDown()
    {
        StopMovement();
    }


    public void StopMovement()
    {
        _moveDirection = 0;
    }

    private void manageWallTimer()
    {
        if (_wallTimer >= 0.15)
        {
            _rigidbody.gravityScale = 3;
            if (_rigidbody.velocity.y <= -4) _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, -4);
        }
    }

    public void Jump()
    {
        switch(CurrentState)
        {
            case State.Grounded:
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce);
                break;
            case State.Cornering:
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce);
                break;
            case State.WallHugging:
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x - _moveDirection * WallReflectionForce, JumpForce);

                if (FacingRight)
                    SetFacingRight(false);
                else
                    SetFacingRight(true);
                break;

        }

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

    }
}