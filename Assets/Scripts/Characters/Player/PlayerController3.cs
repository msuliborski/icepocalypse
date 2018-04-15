
﻿using System.Collections;
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
		EdgeClimbing_L,
		EdgeClimbing_R,
		ObstacleClimbing_R,
		ObstacleClimbing_L
    }

	public State CurrentState = State.Inert;

    private LayerMask Ground;
    private LayerMask Wall;
    private LayerMask Tube;
    private LayerMask TubeStopper;
    private LayerMask HandBar;
    private LayerMask Spikes;
    private LayerMask Slope;
	private LayerMask Edge_TL;
	private LayerMask Edge_TR;
	private LayerMask Edge_BL;
	private LayerMask Edge_BR;



    private Rigidbody2D _rigidbody;
    private Collider2D _collider;


    public bool isScripting;
    public string scriptName;
    public Vector3 scriptDestination1;
    public Vector3 scriptDestination2;
    public float scriptSpeed;
    public Vector3 wallEdge_L0;
    public Vector3 wallEdge_L1;
    public Vector3 wallEdge_L2;
    public Vector3 wallEdge_R0;
    public Vector3 wallEdge_R1;
    public Vector3 wallEdge_R2;
    public bool destinationReached1;
    public bool destinationReached2;
    public bool stopped = false;
    public float playerWidth;
    public float playerHeight;


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

        
        playerWidth = _rigidbody.transform.localScale.x;
        playerHeight = _rigidbody.transform.localScale.y;
		
    }


	void OnCollisionEnter2D(Collision2D collision) {
        if (!isScripting){
        
            if (collision.gameObject.transform.name == "Wall_L"){
                wallEdge_L0 = collision.gameObject.transform.position + new Vector3 (-0.5f - playerWidth/2, collision.gameObject.transform.localScale.y/2 - playerHeight/2, 0);
                wallEdge_L1 = wallEdge_L0 + new Vector3 (0, playerHeight, 0);
                wallEdge_L2 = wallEdge_L1 + new Vector3 (1f, 0, 0);}
            
            if (collision.gameObject.transform.name == "Wall_R"){
                wallEdge_R0 = collision.gameObject.transform.position + new Vector3 (1f, collision.gameObject.transform.localScale.y/2 + playerHeight, 0);
                wallEdge_R1 = wallEdge_R0 + new Vector3 (0, playerHeight, 0);
                wallEdge_R2 = wallEdge_R1 + new Vector3 (-1f, 0, 0);}
            //Debug.Log(wallEdge_L);
        }
    }


	void OnCollisionExit2D(Collision2D coll){

	}

    
    void setScript(string name, Vector3 dest1, float speed){

        scriptName = name;
        scriptDestination1 = dest1;
        scriptSpeed = speed;

        isScripting = true;
    }
    void setScript(string name, Vector3 dest1, Vector3 dest2, float speed){

        scriptName = name;
        scriptDestination1 = dest1;
        scriptDestination2 = dest2;
        scriptSpeed = speed;

        isScripting = true;
    }

    private void script(){
        //if (!stopped) 
        
        if (!stopped) {_rigidbody.isKinematic = true; _rigidbody.velocity = new Vector3 (0, 0, 0); stopped = true;}

        
        switch(scriptName){
            case "single":
                transform.position = Vector3.MoveTowards(_rigidbody.transform.position, new Vector3 (scriptDestination1.x, scriptDestination1.y, scriptDestination1.z), scriptSpeed * Time.deltaTime);
                if (_rigidbody.transform.position == scriptDestination1) {isScripting = false; _rigidbody.isKinematic = false; stopped = false;}
            break;
            case "double":
                if (!destinationReached1) transform.position = Vector3.MoveTowards(_rigidbody.transform.position, scriptDestination1, scriptSpeed * Time.deltaTime);
                if (_rigidbody.transform.position == scriptDestination1) {destinationReached1 = true;}
                if (destinationReached1) transform.position = Vector3.MoveTowards(_rigidbody.transform.position, scriptDestination2, scriptSpeed * Time.deltaTime);
                if (_rigidbody.transform.position == scriptDestination2) destinationReached2 = true;
                if (destinationReached2) { destinationReached1 = false; destinationReached2 = false; isScripting = false; _rigidbody.isKinematic = false; stopped = false; CurrentState = State.Grounded;}
            break;
        }
        
    }
    
    void setBoolFromLayers(){
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
    }
    
    void setStateFromBool(){
            if (_edged_BL && _edged_TL) CurrentState = State.EdgeHanging_L;
            else if (_edged_BR && _edged_TR) CurrentState = State.EdgeHanging_R;
            else if (_edged_TL) CurrentState = State.EdgeClimbing_L;
            else if (_edged_TR) CurrentState = State.EdgeClimbing_R;
            else if (_grounded && _walled) CurrentState = State.Cornering;
            else if (_grounded) CurrentState = State.Grounded;
            else if (_walled) CurrentState = State.WallHugging;
            else CurrentState = State.Inert;
    }

    void Update()
    {   
        if (isScripting)
            script();
        else {

            setBoolFromLayers();
            setStateFromBool();
            switch (CurrentState){
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
                        if ((Input.GetKeyDown(KeyCode.Space))) Jump();}
                    break;
                case State.EdgeHanging_L:
                        setScript("single", wallEdge_L0, 1);
                        _rigidbody.gravityScale = 0;
                        if (Input.GetKey(KeyCode.Space)) {
                            setScript("double", wallEdge_L1, wallEdge_L2, 1);}
                        if (Input.GetKey(KeyCode.DownArrow)) {  _rigidbody.transform.position = new Vector3(_rigidbody.transform.position.x, _rigidbody.transform.position.y - 0.5f, _rigidbody.transform.position.z); 
                                                                _edged_BL = false; _edged_BR = false; _edged_TL = false; _edged_TR = false; }
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
                            transform.position = new Vector3(transform.position.x - 0.3f, transform.position.y, transform.position.z);
                        }
                    }
                    else _rigidbody.velocity = new Vector3(8, 5, 0);

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
		
        if (CurrentState == State.Grounded || CurrentState == State.Sloping) _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce);
        else if (CurrentState == State.Cornering) {setScript("single", new Vector3(transform.position.x, transform.position.y+2, transform.position.z), 10);}
        else if (CurrentState == State.WallHugging)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x - _moveDirection * WallReflectionForce, JumpForce);

            if (FacingRight)
                SetFacingRight(false);
            else
                SetFacingRight(true);
        }
         else if (CurrentState == State.ObstacleClimbing_R || CurrentState == State.ObstacleClimbing_L) _obstacleHasJumped = true;

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



/*
﻿using System.Collections;
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
		EdgeClimbing_L,
		EdgeClimbing_R,
		ObstacleClimbing_R,
		ObstacleClimbing_L
    }

	public State CurrentState = State.Inert;

    private LayerMask Ground;
    private LayerMask Wall;
    private LayerMask Tube;
    private LayerMask TubeStopper;
    private LayerMask HandBar;
    private LayerMask Spikes;
    private LayerMask Slope;
	private LayerMask Edge_TL;
	private LayerMask Edge_TR;
	private LayerMask Edge_BL;
	private LayerMask Edge_BR;



    private Rigidbody2D _rigidbody;
    private Collider2D _collider;


    public bool isScripting;
    public string scriptName;
    public Vector3 scriptDestination1;
    public Vector3 scriptDestination2;
    public float scriptSpeed;
    public Vector3 wallEdge_L0;
    public Vector3 wallEdge_L1;
    public Vector3 wallEdge_L2;
    public Vector3 wallEdge_R0;
    public Vector3 wallEdge_R1;
    public Vector3 wallEdge_R2;
    public bool destinationReached1;
    public bool destinationReached2;
    public bool stopped = false;
    public float playerWidth;
    public float playerHeight;


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

        
        playerWidth = _rigidbody.transform.localScale.x;
        playerHeight = _rigidbody.transform.localScale.y;
		
    }


	void OnCollisionEnter2D(Collision2D collision) {
        if (!isScripting){
        
            if (collision.gameObject.transform.name == "Wall_L"){
                wallEdge_L0 = collision.gameObject.transform.position + new Vector3 (-0.5f - playerWidth/2, collision.gameObject.transform.localScale.y/2 - playerHeight/2, 0);
                wallEdge_L1 = wallEdge_L0 + new Vector3 (0, playerHeight, 0);
                wallEdge_L2 = wallEdge_L1 + new Vector3 (1f, 0, 0);}
            
            if (collision.gameObject.transform.name == "Wall_R"){
                wallEdge_R0 = collision.gameObject.transform.position + new Vector3 (1f, collision.gameObject.transform.localScale.y/2 + playerHeight, 0);
                wallEdge_R1 = wallEdge_R0 + new Vector3 (0, playerHeight, 0);
                wallEdge_R2 = wallEdge_R1 + new Vector3 (-1f, 0, 0);}
            //Debug.Log(wallEdge_L);
        }
    }


	void OnCollisionExit2D(Collision2D coll){

	}

    
    void setScript(string name, Vector3 dest1, float speed){

        scriptName = name;
        scriptDestination1 = dest1;
        scriptSpeed = speed;

        isScripting = true;
    }
    void setScript(string name, Vector3 dest1, Vector3 dest2, float speed){

        scriptName = name;
        scriptDestination1 = dest1;
        scriptDestination2 = dest2;
        scriptSpeed = speed;

        isScripting = true;
    }

    private void script(){
        //if (!stopped) 
        
        if (!stopped) {_rigidbody.isKinematic = true; _rigidbody.velocity = new Vector3 (0, 0, 0); stopped = true;}

        
        switch(scriptName){
            case "single":
                transform.position = Vector3.MoveTowards(_rigidbody.transform.position, new Vector3 (scriptDestination1.x, scriptDestination1.y, scriptDestination1.z), scriptSpeed * Time.deltaTime);
                if (_rigidbody.transform.position == scriptDestination1) {isScripting = false; _rigidbody.isKinematic = false; stopped = false;}
            break;
            case "double":
                if (!destinationReached1) transform.position = Vector3.MoveTowards(_rigidbody.transform.position, scriptDestination1, scriptSpeed * Time.deltaTime);
                if (_rigidbody.transform.position == scriptDestination1) {destinationReached1 = true;}
                if (destinationReached1) transform.position = Vector3.MoveTowards(_rigidbody.transform.position, scriptDestination2, scriptSpeed * Time.deltaTime);
                if (_rigidbody.transform.position == scriptDestination2) destinationReached2 = true;
                if (destinationReached2) { destinationReached1 = false; destinationReached2 = false; isScripting = false; _rigidbody.isKinematic = false; stopped = false; CurrentState = State.Grounded;}
            break;
        }
        
    }
    
    void setBoolFromLayers(){
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
    }
    
    void setStateFromBool(){
            if (_edged_BL && _edged_TL) CurrentState = State.EdgeHanging_L;
            else if (_edged_BR && _edged_TR) CurrentState = State.EdgeHanging_R;
            else if (_edged_TL) CurrentState = State.EdgeClimbing_L;
            else if (_edged_TR) CurrentState = State.EdgeClimbing_R;
            else if (_grounded && _walled) CurrentState = State.Cornering;
            else if (_grounded) CurrentState = State.Grounded;
            else if (_walled) CurrentState = State.WallHugging;
            else CurrentState = State.Inert;
    }

    void Update()
    {   
        if (isScripting)
            script();
        else {

            setBoolFromLayers();
            setStateFromBool();
            switch (CurrentState){
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
                        if ((Input.GetKeyDown(KeyCode.Space))) Jump();}
                    break;
                case State.EdgeHanging_L:
                        setScript("single", wallEdge_L0, 1);
                        _rigidbody.gravityScale = 0;
                        if (Input.GetKey(KeyCode.Space)) {
                            setScript("double", wallEdge_L1, wallEdge_L2, 1);}
                        if (Input.GetKey(KeyCode.DownArrow)) {  _rigidbody.transform.position = new Vector3(_rigidbody.transform.position.x, _rigidbody.transform.position.y - 0.5f, _rigidbody.transform.position.z); 
                                                                _edged_BL = false; _edged_BR = false; _edged_TL = false; _edged_TR = false; }
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
                            transform.position = new Vector3(transform.position.x - 0.3f, transform.position.y, transform.position.z);
                        }
                    }
                    else _rigidbody.velocity = new Vector3(8, 5, 0);

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
		
        if (CurrentState == State.Grounded || CurrentState == State.Sloping) _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce);
        else if (CurrentState == State.Cornering) {setScript("single", new Vector3(transform.position.x, transform.position.y+2, transform.position.z), 10);}
        else if (CurrentState == State.WallHugging)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x - _moveDirection * WallReflectionForce, JumpForce);

            if (FacingRight)
                SetFacingRight(false);
            else
                SetFacingRight(true);
        }
         else if (CurrentState == State.ObstacleClimbing_R || CurrentState == State.ObstacleClimbing_L) _obstacleHasJumped = true;

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

    */


