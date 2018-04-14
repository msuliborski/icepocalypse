using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;


public class PlayerController3 : MonoBehaviour
{
    public UnityEvent TubeStopperDestroy;
    public float JumpForce = 18;
    public float WallReflectionForce = 16;
    public float MoveSpeed = 8;
    public float _wallTimer = 0;
    public bool FacingRight = true;
    float _moveDirection = 0;


    private bool _wallImpact = false;
    private bool _tubeImpact = false;
    private bool _stoppedImpact = false;
    private bool _slopeImpact = false;
    public bool _obstacleHasJumped = false;
    private bool _tubeIgnore = false;

	
	public bool _grounded = false;
	public bool _walled = false;
	public bool _edged_TL = false;
	public bool _edged_TR = false;
	public bool _edged_BL = false;
	public bool _edged_BR = false;
	public bool _spiked = false;
	public bool _handbared = false;
	public bool _sloped = false;
	public bool _tubed = false;
	public bool _tubeStopped = false;


	public enum State
    {
        Inert,
        Grounded,
        Attacking,
        HandBarring,
        Cornering,
        WallHugging,
        TubeSliding,
        TubeStopped,
        Sloping,
		EdgeHanging_L,
		EdgeHanging_R,
		ObstacleClimbing_R,
		ObstacleClimbing_L
		
    }

	public State CurrentState = State.Inert;

    private LayerMask Ground;// = LayerMask.GetMask("Ground");
    private LayerMask Wall;// = LayerMask.GetMask("Wall");
    private LayerMask Tube;// = LayerMask.GetMask("Tube");
    private LayerMask TubeStopper;// = LayerMask.GetMask("Ground");
    private LayerMask HandBar;// = LayerMask.GetMask("Handbar");
    private LayerMask Spikes;// = LayerMask.GetMask("Spikes");
    private LayerMask Slope;// = LayerMask.GetMask("Slope");
	private LayerMask Edge_TL;// = LayerMask.GetMask("Edge_TL");
	private LayerMask Edge_TR;// = LayerMask.GetMask("Edge_TR");
	private LayerMask Edge_BL;// = LayerMask.GetMask("Edge_BL");
	private LayerMask Edge_BR;// = LayerMask.GetMask("Edge_BR");



    private Rigidbody2D _rigidbody;
    private Collider2D _collider;



    // Use this for initialization
    void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _collider = gameObject.GetComponent<Collider2D>();
		Ground = LayerMask.GetMask("Ground");
		Wall = LayerMask.GetMask("Wall");
		Tube = LayerMask.GetMask("Tube");
		TubeStopper = LayerMask.GetMask("Stopper");
		HandBar = LayerMask.GetMask("Handbar");
		Spikes = LayerMask.GetMask("Spikes");
		Slope = LayerMask.GetMask("Slope");
		Edge_TL = LayerMask.GetMask("Edge_TL");
		Edge_TR = LayerMask.GetMask("Edge_TR");
		Edge_BL = LayerMask.GetMask("Edge_BL");
		Edge_BR = LayerMask.GetMask("Edge_BR");
		
    }

	
	void OnCollisionEnter2D(Collision2D coll)
	{
		
	}


	void OnCollisionExit2D(Collision2D coll)
	{

	}


    void Update()
    {   

		_rigidbody.isKinematic = true;
		float step = 2 * Time.deltaTime;
         transform.position = Vector3.MoveTowards(transform.position, new Vector3 (transform.position.x, transform.position.y + 3, transform.position.z), step);
        if (Physics2D.IsTouchingLayers(_collider, Ground)) _grounded = true; else _grounded = false;
        if (Physics2D.IsTouchingLayers(_collider, Wall)) _walled = true; else _walled = false;
        if (Physics2D.IsTouchingLayers(_collider, Slope)) _sloped = true; else _sloped = false;
        if (Physics2D.IsTouchingLayers(_collider, HandBar)) _handbared = true; else _handbared = false;
        if (Physics2D.IsTouchingLayers(_collider, Tube)) _tubed = true; else _tubed = false;
        if (Physics2D.IsTouchingLayers(_collider, TubeStopper)) _tubeStopped = true; else _tubeStopped = false;
        if (Physics2D.IsTouchingLayers(_collider, Edge_BL)) _edged_BL = true; else _edged_BL = false;
        if (Physics2D.IsTouchingLayers(_collider, Edge_BR)) _edged_BR = true; else _edged_BR = false;
        if (Physics2D.IsTouchingLayers(_collider, Edge_TL)) _edged_TL = true; else _edged_TL = false;
        if (Physics2D.IsTouchingLayers(_collider, Edge_TR)) _edged_TR = true; else _edged_TR = false;


		if (_edged_BL && _edged_TL) CurrentState = State.EdgeHanging_L;
		else if (_edged_BR && _edged_TR) CurrentState = State.EdgeHanging_R;
		else if (_grounded && _walled) CurrentState = State.Cornering;
		else if (_grounded) CurrentState = State.Grounded;
		else if (_walled) CurrentState = State.WallHugging;
		else CurrentState = State.Inert;




        switch (CurrentState)
        {


            case State.Grounded:
                Movement();
                resetImpacts();
                _tubeIgnore = false;
                break;

            case State.Cornering:
                Movement();
                resetImpacts();
                break;

            case State.Inert:
                resetImpacts();
				_rigidbody.gravityScale = 10;
                break;

            case State.WallHugging:
				if (_rigidbody.velocity.y < 0 || _wallImpact){
					_wallTimer += Time.deltaTime;
					onWallImpact();
					manageWallTimer();
					if ((Input.GetKeyDown(KeyCode.Space))) Jump();
				}
                break;
            case State.EdgeHanging_L:
					_rigidbody.velocity = new Vector2(0, 0);
					_rigidbody.gravityScale = 0;
					if (Input.GetKey(KeyCode.Space)) Jump();
					if (Input.GetKey(KeyCode.DownArrow)) {_rigidbody.transform.position = new Vector3(_rigidbody.transform.position.x, _rigidbody.transform.position.y - 1, _rigidbody.transform.position.z);}
                break;

            case State.Sloping:
                onSlopeImpact();
                transform.rotation = Quaternion.Euler(0, 0, -30);
                if ((Input.GetKeyDown(KeyCode.Space))) Jump();
                break;

           case State.ObstacleClimbing_L:

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

            case State.ObstacleClimbing_R:

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

            case State.TubeSliding:

                onTubeImpact();

                if (_rigidbody.velocity.y <= -3) _rigidbody.velocity = new Vector2(0, -3);

                break;

            case State.TubeStopped:


                onTubeStopperImpact();

                if (Input.GetKeyDown(KeyCode.DownArrow)) OnKeyDown();

                break;




        }
    }

    public void OnKeyDown()
    {
        switch (CurrentState)
        {
            case State.TubeStopped:
                _rigidbody.gravityScale = 10;
                TubeStopperDestroy.Invoke();
                _tubeIgnore = true;
                break;
            
            default:
                StopMovement();
                break;
        }
    }


    private void Movement()
    {
		if (Input.GetKeyDown(KeyCode.RightArrow)) SetFacingRight(true);
		if (Input.GetKeyDown(KeyCode.LeftArrow)) SetFacingRight(false);
		if (Input.GetKeyDown(KeyCode.DownArrow)) OnKeyDown();
		if (Input.GetKeyDown(KeyCode.Space)) Jump();
        _rigidbody.velocity = new Vector2(MoveSpeed * _moveDirection, _rigidbody.velocity.y);
    }

    public void StopMovement()
    {
        _moveDirection = 0;
    }

    public void Jump()
    {
		/*
        if (CurrentState == State.Grounded || CurrentState == State.Sloping || CurrentState == State.Cornering) _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce);
        else if (CurrentState == State.WallHugging)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x - _moveDirection * WallReflectionForce, JumpForce);

            if (FacingRight)
                SetFacingRight(false);
            else
                SetFacingRight(true);
        }
         else if (CurrentState == State.ObstacleClimbing_R || CurrentState == State.ObstacleClimbing_L) _obstacleHasJumped = true;
*/

		float step = 2 * Time.deltaTime;
         transform.position = Vector3.MoveTowards(transform.position, new Vector3 (transform.position.x, transform.position.y + 3, transform.position.z), step);



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


    private void manageWallTimer()
    {
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
            
 ;           //_rigidbody.constraints = RigidbodyConstraints2D.None;
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
        //_rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        _obstacleHasJumped = false;
        _rigidbody.isKinematic = false;
        _obstacleHasJumped = false;
     }
}



