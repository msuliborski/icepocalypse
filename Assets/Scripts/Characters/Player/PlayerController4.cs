using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;


public class PlayerController4 : MonoBehaviour
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



    public bool _grounded = false;
    public bool _walled = false;
    public bool _inert = false;


    private Rigidbody2D _rigidbody;
    private Collider2D _collider;



    // Use this for initialization
    void Start()
    {

        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _collider = gameObject.GetComponent<Collider2D>();
    }


    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
           _walled = true;
            if (!_grounded)
            {
                _rigidbody.velocity = new Vector2(0, 0);
                _rigidbody.gravityScale = 0;
            }


        }
        else if (coll.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _grounded = true;
            Debug.Log("GRAUNDD!");

        }



    }
    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _grounded = false;
            if (_walled)
            {
                _rigidbody.velocity = new Vector2(0, 0);
                _rigidbody.gravityScale = 0;
            }
        }
        else if (coll.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            _walled = false;
            _rigidbody.gravityScale = 10;
            _wallTimer = 0;

        }

    }


    void Update()
    {

        if (_grounded) CurrentState = State.Grounded;
        else if (_walled) CurrentState = State.WallHugging;
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
        switch (CurrentState)
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