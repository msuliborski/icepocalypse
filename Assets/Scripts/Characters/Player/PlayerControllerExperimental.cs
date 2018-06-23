using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using Scripts.Variables;


public class PlayerControllerExperimental : MonoBehaviour
{
    #region Variables
    public GameEvent PlayerHealEvent;
    public GameEvent PlayerDamageEvent;
    public GameEvent PlayerDeathEvent;
    public float JumpForce = 18;
    public float WallJumpForce = 24;
    public float WallReflectionForce = 17;
    public float MoveSpeed = 10;
    public int Hp = 100;
    private float _wallTimer = 0;
    private float _inertTimer = 0;
    public bool FacingRight = true;
    private float _moveDirection = 0;
    public bool InEndOfTube = false;
    private bool _ignoreLedderEdge = false;
    private bool _inertDown = false;
    private List<Vector3> _scriptDestinations;
    private float _scriptSpeed;
    public bool _isScripting = false;
    private bool _animListExecuted = false;
    private int _index;
    private int _indexToReact = -1;
    private List<Vector3> _rendererPosDif;
    private bool _gameStarted = false;

    private enum animScriptCommands
    {
        Nothing,
        LadderMovementTrue,
        LadderMovementFalse,
        LadderFalse,
        LadderTrue,
        MovementTrue,
        MovementFalse,
        TubeTrue,
        TubeFalse,
        TubeIdleTrue,
        TubeIdleFalse,
        InertDownTrue,
        InertDownFalse,
        SlopeTrue,
        SlopeFalse,
        CzekaningTrue,
        CzekaningFalse,
        JumpToWallTrue,
        JumpToWallFalse,
        UpLadderTrue,
        UpLadderFalse,
        UpWallTrue,
        UpWallFalse,
        InEndOfTubeTrue

    }

    private List<List<animScriptCommands>> _animScriptCommands;

    public enum PlayerState
    {
        Inert,
        Grounded,
        Attacking,
        WallClimbing,
        WallHugging,
        EgdeClimbingBody,
        EgdeClimbingCorner,
        TubeSliding,
        Sloping,
        Laddering,
        EdgeLaddering, // moving up
        EdgeLaddering1, // moving down
        Death
    }

    public PlayerState PreviousState = PlayerState.Inert;
    public PlayerState CurrentState = PlayerState.Inert;
    private LayerMask Ground;
    private LayerMask Wall;
    private LayerMask Tube;
    private LayerMask Obstacle_R;
    private LayerMask Obstacle_L;
    private LayerMask HandBar;
    private LayerMask Slope;
    private LayerMask Edge;
    private LayerMask Ladder;
    private LayerMask LadderEdge;
    private LayerMask LadderEdge1;
    private GameObject _renderer;
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private Collider2D _colliderWhole;
    private Collider2D _colliderBody;
    private Collider2D _colliderLegs;
    private Collider2D _colliderCorner;
    private float tubeHeight;
    private float playerWidth;
    private float playerHeight;
    bool _onLeftDirection;
    bool _onRightDirection;
    bool _onTopDirection;
    bool _onDownDirection;
    private bool _scriptInputBlocade = true;
    #endregion

    #region Start&Update
    void Start()
     {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _renderer = gameObject.transform.Find("SpriteAnimation").gameObject;
        _animator = _renderer.GetComponent<Animator>();
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
        Ladder = LayerMask.GetMask("Ladder");
        LadderEdge = LayerMask.GetMask("LadderEdge");
        LadderEdge1 = LayerMask.GetMask("LadderEdge1");
        playerWidth = 1f;
        playerHeight = 2f;
        _scriptDestinations = new List<Vector3>();
        _animScriptCommands = new List<List<animScriptCommands>>();
        _rendererPosDif = new List<Vector3>();

        _onLeftDirection = false;
        _onRightDirection = false;
        _onTopDirection = false;
        _onDownDirection = false;
    }

    public void OnLeftDirection() 
    {
        if (_isScripting)
        {
            if (!_scriptInputBlocade) _onLeftDirection = true;
        }
        else
        {
            if (_gameStarted && CurrentState == PlayerState.Grounded)
            {
                _onLeftDirection = true;
            }
        }
    }

    public void OnRightDirection() 
    {
        if (_isScripting)
        {
            if (!_scriptInputBlocade) _onRightDirection = true;
        }
        else
        {
            if (_gameStarted && CurrentState == PlayerState.Grounded)
            {
                _onRightDirection = true;
            }
        }
    }

    public void OnTopDirection()
    {
        if (_isScripting)
        {
            if (!_scriptInputBlocade) _onTopDirection = true;
        }
        else
        {
            if (_gameStarted && (CurrentState == PlayerState.Grounded || CurrentState == PlayerState.Laddering || CurrentState == PlayerState.WallHugging))
            {
                _onTopDirection = true;
            }
        }
    }

    public void OnDownDirection()
    {
        if (_isScripting)
        {
            if (!_scriptInputBlocade) _onTopDirection = true;
        }
        else
        {
            if (_gameStarted && (CurrentState == PlayerState.Grounded || CurrentState == PlayerState.Laddering))
            {
                _onTopDirection = true;
            }
        }
    }

    public void OnGameStarted()
    {
        Debug.Log("Player received signal, let's go");
        _gameStarted = true;
    }

    public void OnBarrelExploded()
    {
        if (!_gameStarted) return;

        TakeHP(10);   
    }

    public void TakeHP(int amount)
    {
        Hp -= amount;
        for (int i = amount / 10; i >= 1; i--)
        {
            PlayerDamageEvent.Raise();
        }

        if (Hp <= 0)
        {
            PlayerDeathEvent.Raise();
            _gameStarted = false;
        }
    }

    public void OnEatFood()
    {
        Heal(10);
    }

    public void Heal(int amount)
    {
        if (Hp >= 100)
            return;
        
        if (Hp + amount > 100)
        {
            amount = 100 - Hp;
        }
        Hp += amount;
        for (int i = amount / 10; i >= 1; i--)
        {
            PlayerHealEvent.Raise();
        }

    }

    void Update()
    {
        if (!_gameStarted || CurrentState == PlayerState.Attacking)
            return;
        
        if (Input.GetKeyDown(KeyCode.P)) Debug.Break();

        if (_isScripting) script();
        else
        {
            if (Hp > 0)
            {
                if (Physics2D.IsTouchingLayers(_colliderBody, Edge))
                {
                    if (CurrentState != PlayerState.EgdeClimbingBody)
                    {
                        PreviousState = CurrentState;
                        onEdgeBodyImpact();
                    }
                    CurrentState = PlayerState.EgdeClimbingBody;
                }
                else if (Physics2D.IsTouchingLayers(_colliderCorner, Edge))
                {
                    if (CurrentState != PlayerState.EgdeClimbingCorner)
                    {
                        PreviousState = CurrentState;
                        onEdgeCornerImpact();
                    }
                    CurrentState = PlayerState.EgdeClimbingCorner;
                }
                else if ((Physics2D.IsTouchingLayers(_colliderCorner, LadderEdge) || Physics2D.IsTouchingLayers(_colliderBody, LadderEdge)) && !_ignoreLedderEdge)
                {
                    if (CurrentState != PlayerState.EdgeLaddering)
                    {
                        PreviousState = CurrentState;
                        onLadderEdgeImpact();
                    }
                    CurrentState = PlayerState.EdgeLaddering;
                }
                else if (Physics2D.IsTouchingLayers(_colliderLegs, LadderEdge1))
                {
                    if (CurrentState != PlayerState.EdgeLaddering1)
                    {
                        PreviousState = CurrentState;
                        onLadderEdge1Impact();
                    }
                    CurrentState = PlayerState.EdgeLaddering1;
                }
                else if (Physics2D.IsTouchingLayers(_colliderBody, Ladder))
                {
                    if (CurrentState != PlayerState.Laddering)
                    {
                        PreviousState = CurrentState;
                        onLadderImpact();
                    }
                    CurrentState = PlayerState.Laddering;
                }
                else if (Physics2D.IsTouchingLayers(_colliderLegs, Ground))
                {
                    if (CurrentState != PlayerState.Grounded)
                    {
                        PreviousState = CurrentState;
                        onGroundImpact();
                    }
                    CurrentState = PlayerState.Grounded;
                }
                else if (Physics2D.IsTouchingLayers(_colliderWhole, Tube))
                {
                    if (CurrentState != PlayerState.TubeSliding)
                    {
                        PreviousState = CurrentState;
                        onTubeImpact();
                    }
                    CurrentState = PlayerState.TubeSliding;
                }
                else if (Physics2D.IsTouchingLayers(_colliderBody, Wall))
                {
                    if (CurrentState != PlayerState.WallHugging)
                    {
                        PreviousState = CurrentState;
                        onWallImpact();
                    }
                    CurrentState = PlayerState.WallHugging;
                }
                else if (Physics2D.IsTouchingLayers(_colliderWhole, Slope))
                {
                    if (CurrentState != PlayerState.Sloping)
                    {
                        PreviousState = CurrentState;
                        onSlopeImpact();
                    }
                    CurrentState = PlayerState.Sloping;
                }
                else
                {
                    if (CurrentState != PlayerState.Inert)
                    {
                        PreviousState = CurrentState;
                        onBeingInert();
                    }
                    CurrentState = PlayerState.Inert;
                }
            }
            else
            {
                if (CurrentState != PlayerState.Death) PreviousState = CurrentState;
                CurrentState = PlayerState.Death;
            }

            switch (CurrentState)
            {
                case PlayerState.Grounded:

                    movement();
                    if (Input.GetKeyDown(KeyCode.RightArrow) || _onRightDirection)  
                    {
                        OnKeyRight();
                        _onRightDirection = false;
                    }
                    if (Input.GetKeyDown(KeyCode.LeftArrow) || _onLeftDirection) 
                    {
                        OnKeyLeft();
                        _onLeftDirection = false;
                    }
                    if (Input.GetKeyDown(KeyCode.DownArrow) || _onDownDirection)
                    {
                        _onDownDirection = false;
                        OnKeyDown();
                    } 
                    if (Input.GetKeyDown(KeyCode.Space) || _onTopDirection)
                    {
                        if (_moveDirection == 0 && !Physics2D.IsTouchingLayers(_colliderBody, Wall)) _animator.SetBool("JumpIdle", true);
                        _onTopDirection = false;
                        OnKeySpace();
                    } 
                    if (Physics2D.IsTouchingLayers(_colliderBody, Wall)) _animator.SetBool("Movement", false);
                    else _animator.SetBool("Movement", (_moveDirection != 0) ? true : false);
                    break;

                case PlayerState.Inert:

                    /*if (!_inertDown && _rigidbody.velocity.y < -5f)
                    {
                        _inertDown = true;
                        _animator.SetBool("InertDown2", true);
                    }*/
                    _inertTimer += Time.deltaTime;
                    break;

                case PlayerState.WallHugging:
                   
                    manageWallTimer();
                    if (Input.GetKeyDown(KeyCode.Space) || _onTopDirection)
                    {
                        _onTopDirection = false;
                        OnKeySpace();
                    } 
                    break;

                case PlayerState.Laddering:

                    if (_rigidbody.velocity.y <= 0 && Physics2D.IsTouchingLayers(_colliderLegs, Ground)) _animator.SetBool("LadderMovement", false);
                    if (Input.GetKeyDown(KeyCode.Space) || _onTopDirection)
                    {
                        _onTopDirection = false;
                        OnKeySpace();
                    } 
                    if (Input.GetKeyDown(KeyCode.DownArrow) || _onDownDirection)
                    {
                        _onDownDirection = false;
                        OnKeyDown();
                    } 
                    if ((Input.GetKeyDown(KeyCode.RightArrow) || _onRightDirection) && !FacingRight)
                    {
                        OnKeyRight();
                        _onRightDirection = false;
                    }
                    if ((Input.GetKeyDown(KeyCode.LeftArrow) || _onLeftDirection) && FacingRight) 
                    {
                        OnKeyLeft();
                        _onLeftDirection = false;
                    }
                    break;
                
                case PlayerState.Sloping:

                    if (Input.GetKeyDown(KeyCode.Space) || _onTopDirection)
                    {
                        _onTopDirection = false;
                        OnKeySpace();
                    }
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
        if (_isScripting)
        {
            _animListExecuted = false;
            switch (CurrentState)
            {
                case PlayerState.EdgeLaddering:
                    _scriptInputBlocade = true;
                    _isScripting = false;
                    _rigidbody.isKinematic = false;
                    if (FacingRight) _rigidbody.velocity = new Vector2(0.5f, -3f);
                    else _rigidbody.velocity = new Vector2(-0.5f, -3f);
                    _ignoreLedderEdge = true;
                    break;

                case PlayerState.TubeSliding:
                    _scriptInputBlocade = true;
                    _animator.SetBool("Tube Idle", false);
                    _animator.SetBool("Tube", false);
                    _animator.SetBool("Inert Down", true);
                    _index++;
                    break;

            }
        }
        else
        {
            switch (CurrentState)
            {
                case PlayerState.Laddering:
                    MoveDownOnLadder();
                    break;
                default:
                    StopMovement();
                    break;
            }
        }
    }

    public void OnKeyRight()
    {
        if (_isScripting)
        {
            _animListExecuted = false;
            switch (CurrentState)
            {
                case PlayerState.EdgeLaddering:
                    if (!FacingRight)
                    {
                        _scriptInputBlocade = true;
                        _isScripting = false;
                        _rigidbody.isKinematic = false;
                        _rigidbody.velocity = new Vector2(3f, 0);
                        SetFacingRight(true);
                        _ignoreLedderEdge = true;
                        _animator.SetBool("Ladder", false);
                        _animator.SetBool("LadderMovement", false);
                    }
                    break;
            }
        }
        else
        {
            switch (CurrentState)
            {
                case PlayerState.Laddering:
                    MoveRightOnLadder();
                    break;
                default:
                    SetFacingRight(true);
                    break;
            }
        }
    }

    public void OnKeyLeft()
    {
        if (_isScripting)
        {
            _animListExecuted = false;
            switch (CurrentState)
            {
                case PlayerState.EdgeLaddering:
                    if (FacingRight)
                    {
                        _scriptInputBlocade = true;
                        _isScripting = false;
                        _rigidbody.isKinematic = false;
                        _rigidbody.velocity = new Vector2(-3f, 0);
                        SetFacingRight(false);
                        _ignoreLedderEdge = true;
                        _animator.SetBool("Ladder", false);
                        _animator.SetBool("LadderMovement", false);
                    }
                    break;
            }
        }
        else
        {
            switch (CurrentState)
            {
                case PlayerState.Laddering:
                    MoveLeftOnLadder();
                    break;
                default:
                    SetFacingRight(false);
                    break;
            }
        }
    }

    public void OnKeySpace()
    {
        if (_isScripting)
        {
            _animListExecuted = false;
            switch (CurrentState)
            {
                case PlayerState.EdgeLaddering:
                    _scriptInputBlocade = true;
                    _index++;
                    _animator.SetBool("UpLadder", true);
                    break;

                case PlayerState.EgdeClimbingCorner:
                    _scriptInputBlocade = true;
                    _index++;
                    _animator.SetBool("UpWall", true);
                    break;
            }
        }
        else
        {
            if (CurrentState == PlayerState.Grounded) _rigidbody.AddForce(new Vector2(0, 450*JumpForce));//_rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce);
            else if (CurrentState == PlayerState.WallHugging)
            {
                if (FacingRight)
                {
                    _rigidbody.AddForce(new Vector2(-300*WallReflectionForce, 400*WallJumpForce));
                    SetFacingRight(false);
                }
                else
                {
                    _rigidbody.AddForce(new Vector2(300 * WallReflectionForce, 400 * WallJumpForce));
                    SetFacingRight(true);
                }
                _animator.SetBool("Czekaning", false);
                _animator.SetBool("JumpToWall", false);
                _animator.SetBool("Movement", true);
                _animator.SetBool("WallReflection", true);
            }
            else if (CurrentState == PlayerState.Laddering) MoveUpOnLadder();
        }
    }

    public void SetFacingRight(bool facingRight)
    {
        if (facingRight)
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

    private void movement()
    {
        _rigidbody.velocity = new Vector2(MoveSpeed * _moveDirection, _rigidbody.velocity.y);
    }

    public void StopMovement()
    {
        _moveDirection = 0;
    }

    public void MoveLeftOnLadder()
    {
        _rigidbody.velocity = new Vector2(-3f, _rigidbody.velocity.y);
        SetFacingRight(false);
    }

    public void MoveRightOnLadder()
    {
        _rigidbody.velocity = new Vector2(3f, _rigidbody.velocity.y);
        SetFacingRight(true);
    }

    public void MoveDownOnLadder()
    {
        _rigidbody.velocity = new Vector2(0f, -3f);
        _animator.SetBool("LadderMovement", true);
    }

    public void MoveUpOnLadder()
    {
        _rigidbody.velocity = new Vector2(0f, 3f);
        _animator.SetBool("LadderMovement", true);
    }
    #endregion

    #region Impacts

    private void onBeingInert()
    {
        if (PreviousState == PlayerState.WallHugging) StartCoroutine(InertTimerZero());
        if (!_ignoreLedderEdge)
        {
            _animator.SetBool("Ladder", false);
            _animator.SetBool("LadderMovement", false);
        }
        _renderer.transform.position = new Vector2(transform.position.x, transform.position.y);
        _wallTimer = 0;
        _rigidbody.gravityScale = 10;
    }

    private void onGroundImpact()
    {
        InEndOfTube = false;
        _inertDown = false;
        _ignoreLedderEdge = false;
        _animator.SetBool("JumpIdle", false);
        _animator.SetBool("Ladder", false);
        _animator.SetBool("LadderMovement", false);
        _animator.SetBool("Inert Down", false);
        _animator.SetBool("WallReflection", false);
        _animator.SetBool("JumpToWall", false);
        _animator.SetBool("Czekaning", false);
        _animator.SetBool("Slope", false);
        _renderer.transform.position = new Vector2(transform.position.x, transform.position.y);
        _wallTimer = 0;
        _rigidbody.gravityScale = 10;
        for (float i = 0.55f; i < _inertTimer; i += 0.55f) TakeHP(10);
        _inertTimer = 0;
    }

    private void onLadderImpact()
    {
        _animator.SetBool("WallReflection", false);
        _animator.SetBool("Ladder", true);
        _rigidbody.gravityScale = 0;
        if (!_ignoreLedderEdge) _rigidbody.velocity = new Vector2(0f, 0f);
        else
        {
            _ignoreLedderEdge = false;
            _rigidbody.velocity = new Vector2(0f, -3f);
            _animator.SetBool("LadderMovement", true);
        }
        _inertTimer = 0;
    }

    private void onSlopeImpact()
    {
        _animator.SetBool("Tube Idle", false);
        _animator.SetBool("Ladder", false);
        _animator.SetBool("LadderMovement", false);
        _animator.SetBool("Tube", false);
        _animator.SetBool("Inert Down", false);
        _animator.SetBool("Movement", false);
        _animator.SetBool("WallReflection", false);
        _animator.SetBool("Czekaning", false);
        _animator.SetBool("Slope", true);
        GameObject go = findClosestObjectWithTag("slopeEnd", 5);
        if (go.transform.localScale.x > 0) SetFacingRight(true);
        else SetFacingRight(false);
        _renderer.transform.position = new Vector2(-0.25f + transform.position.x, -0.25f + transform.position.y);
        _rigidbody.gravityScale = 18;
        for (float i = 0.55f; i < _inertTimer; i += 0.55f) TakeHP(10);
        _inertTimer = 0;
    }


    private void onTubeImpact()
    {
        resetScripting();
        _animator.SetBool("Ladder", false);
        _animator.SetBool("LadderMovement", false);
        _animator.SetBool("Tube", false);
        _animator.SetBool("Inert Down", false);
        _animator.SetBool("Movement", false);
        _animator.SetBool("WallReflection", false);
        _animator.SetBool("Czekaning", false);
        _animator.SetBool("Slope", false);
        GameObject Start = findClosestObjectWithTag("tubeMarker", 1);
        GameObject End = findClosestObjectWithTag("tubeEnd", 1);
        if (transform.position.x < Start.transform.position.x) SetFacingRight(true);
        else SetFacingRight(false);
        _scriptDestinations.Add(new Vector3(Start.transform.position.x, transform.position.y, 0f));
        _scriptDestinations.Add(new Vector3(End.transform.position.x + playerWidth, End.transform.position.y + playerHeight / 4, 0f));
        _scriptSpeed = 3.0f;
        _indexToReact = 1;
        _animScriptCommands.Add(new List<animScriptCommands> { animScriptCommands.MovementFalse, animScriptCommands.TubeTrue });
        _animScriptCommands.Add(new List<animScriptCommands> { animScriptCommands.TubeIdleTrue, animScriptCommands.InEndOfTubeTrue });
    }

    void onEdgeCornerImpact()
    {
        resetScripting();
        _animator.SetBool("WallReflection", false);
        GameObject go = findClosestObjectWithTag("WallEdge", 1);
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
        _indexToReact = 0;
        _animScriptCommands.Add(new List<animScriptCommands> {animScriptCommands.JumpToWallFalse, animScriptCommands.CzekaningTrue});
        _animScriptCommands.Add(new List<animScriptCommands> { animScriptCommands.UpWallFalse,  animScriptCommands.CzekaningFalse, animScriptCommands.MovementTrue});
        _animScriptCommands.Add(new List<animScriptCommands> {  });
        _rendererPosDif.Add(new Vector3(0f, 0f, 0f));
        if (FacingRight) _rendererPosDif.Add(new Vector3(0.15f, -0.15f, 0f));
        else _rendererPosDif.Add(new Vector3(-0.15f, -0.15f, 0f));
        _rendererPosDif.Add(new Vector3(0f, 0f, 0f));
    }

    void onLadderEdgeImpact()
    {
        resetScripting();
        _animator.SetBool("LadderMovement", false);
        _animator.SetBool("WallReflection", false);
        _animator.SetBool("Ladder", true);
        GameObject go = findClosestObjectWithTag("ladderEdge", 5);
        if (FacingRight)
        {
            _scriptDestinations.Add(go.transform.position + new Vector3(-1f, go.transform.localScale.y / 2 - 1f, 0f));
            _scriptDestinations.Add(_scriptDestinations[0] + new Vector3(0, playerHeight, 0));
            _scriptDestinations.Add(_scriptDestinations[1] + new Vector3(2f, 0, 0));
        }
        else
        {
            _scriptDestinations.Add(go.transform.position + new Vector3(1f, go.transform.localScale.y / 2 - 1f, 0f));
            _scriptDestinations.Add(_scriptDestinations[0] + new Vector3(0, playerHeight, 0));
            _scriptDestinations.Add(_scriptDestinations[1] + new Vector3(-2f, 0, 0));
        }
        _scriptSpeed = 4.0f;
        _indexToReact = 0;
        _animScriptCommands.Add(new List<animScriptCommands> { });
        _animScriptCommands.Add(new List<animScriptCommands> { animScriptCommands.LadderFalse, animScriptCommands.UpLadderFalse });//animScriptCommands.MovementTrue });
        _animScriptCommands.Add(new List<animScriptCommands> {  });
        _rendererPosDif.Add(new Vector3(0f, 0f, 0f));
        if (FacingRight) _rendererPosDif.Add(new Vector3(0.15f, -0.15f, 0f));
        else _rendererPosDif.Add(new Vector3(-0.15f, -0.15f, 0f));
        _rendererPosDif.Add(new Vector3(0f, 0f, 0f));
    }


    void onLadderEdge1Impact()
    {
        resetScripting();
        GameObject go = findClosestObjectWithTag("ladderEdge", 5);
        if (FacingRight)
        {
            SetFacingRight(false);
            _scriptDestinations.Add(go.transform.position + new Vector3(1f, go.GetComponent<SpriteRenderer>().bounds.size.y / 2 + playerHeight / 2));
            _scriptDestinations.Add(_scriptDestinations[0] + new Vector3(0f, -playerWidth));
        }
        else
        {
            SetFacingRight(true);
            _scriptDestinations.Add(go.transform.position + new Vector3(-1f, go.GetComponent<SpriteRenderer>().bounds.size.y / 2 + playerHeight/2));
            _scriptDestinations.Add(_scriptDestinations[0] + new Vector3(0f, -playerWidth));
        }
        _scriptSpeed = 4.0f;
        _indexToReact = -1;
        _animScriptCommands.Add(new List<animScriptCommands> { });
        _animScriptCommands.Add(new List<animScriptCommands> { animScriptCommands.LadderTrue });
    }

    void onEdgeBodyImpact()
    {
        resetScripting();
        _animator.SetBool("WallReflection", false);
        GameObject go = findClosestObjectWithTag("WallEdge", 1);
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
        _indexToReact = -1;
    }

    private void onWallImpact()
    {
        if (PreviousState == PlayerState.Grounded)
        {
            _rigidbody.velocity = new Vector2(0, 6);
            _animator.SetBool("JumpToWall", true);
        }
        else
        {
            _rigidbody.velocity = new Vector2(0, 0);
            _animator.SetBool("Slope", false);
            _animator.SetBool("WallReflection", false);
            _animator.SetBool("Czekaning", true);
        }
        _rigidbody.gravityScale = 0;
        _inertTimer = 0;
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
            _renderer.transform.position = transform.position;
            return;
        }
        transform.position = Vector2.MoveTowards(_rigidbody.transform.position, _scriptDestinations[_index], _scriptSpeed * Time.deltaTime);
        if (_index < _rendererPosDif.Count)
        {
            _renderer.transform.position = _rendererPosDif[_index] + transform.position;
        }
        if (_rigidbody.transform.position == _scriptDestinations[_index])
        {

            if (_index < _animScriptCommands.Count && !_animListExecuted)
            {
                _animListExecuted = true;
                foreach (var animScriptCommand in _animScriptCommands[_index])
                {
                    switch (animScriptCommand)
                    {
                        case animScriptCommands.LadderMovementFalse:
                            _animator.SetBool("LadderMovement", false);
                            break;

                        case animScriptCommands.LadderMovementTrue:
                            _animator.SetBool("LadderMovement", true);
                            break;

                        case animScriptCommands.LadderFalse:
                            _animator.SetBool("Ladder", false);
                            break;

                        case animScriptCommands.LadderTrue:
                            _animator.SetBool("Ladder", true);
                            break;

                        case animScriptCommands.MovementFalse:
                            _animator.SetBool("Movement", false);
                            break;

                        case animScriptCommands.MovementTrue:
                            _animator.SetBool("Movement", true);
                            break;

                        case animScriptCommands.TubeFalse:
                            _animator.SetBool("Tube", false);
                            break;

                        case animScriptCommands.TubeTrue:
                            _animator.SetBool("Tube", true);
                            break;

                        case animScriptCommands.InertDownFalse:
                            _animator.SetBool("Inert Down", false);
                            break;

                        case animScriptCommands.InertDownTrue:
                            _animator.SetBool("Inert Down", true);
                            break;

                        case animScriptCommands.SlopeFalse:
                            _animator.SetBool("Slope", false);
                            break;

                        case animScriptCommands.SlopeTrue:
                            _animator.SetBool("Slope", true);
                            break;

                        case animScriptCommands.TubeIdleFalse:
                            _animator.SetBool("Tube Idle", false);
                            break;

                        case animScriptCommands.TubeIdleTrue:
                            _animator.SetBool("Tube Idle", true);
                            break;

                        case animScriptCommands.CzekaningFalse:
                            _animator.SetBool("Czekaning", false);
                            break;

                        case animScriptCommands.CzekaningTrue:
                            _animator.SetBool("Czekaning", true);
                            break;

                        case animScriptCommands.JumpToWallFalse:
                            _animator.SetBool("JumpToWall", false);
                            break;

                        case animScriptCommands.JumpToWallTrue:
                            _animator.SetBool("JumpToWall", true);
                            break;

                        case animScriptCommands.UpLadderTrue:
                            _animator.SetBool("UpLadder", true);
                            break;

                        case animScriptCommands.UpLadderFalse:
                            _animator.SetBool("UpLadder", false);
                            break;

                        case animScriptCommands.UpWallTrue:
                            _animator.SetBool("UpWall", true);
                            break;

                        case animScriptCommands.UpWallFalse:
                            _animator.SetBool("UpWall", false);
                            break;

                        case animScriptCommands.InEndOfTubeTrue:
                            InEndOfTube = true;
                            break;
                    }
                }
            }

            if (_indexToReact == _index)
            {
                _scriptInputBlocade = false;

                if (_onTopDirection || Input.GetKeyDown(KeyCode.Space))
                {
                    OnKeySpace();
                    _onTopDirection = false;
                }
                else if (_onDownDirection || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    OnKeyDown();
                    _onDownDirection = false;
                } 
                else if (_onRightDirection || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    OnKeyRight();
                    _onRightDirection = false;
                }
                else if (_onLeftDirection || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    OnKeyLeft();
                    _onLeftDirection = false;
                }
            }
            else
            {
                _animListExecuted = false;
                _index++;
            }
            

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


    private void resetScripting()
    {
        _scriptDestinations.Clear();
        _isScripting = true;
        _rigidbody.velocity = new Vector3(0, 0, 0);
        _rigidbody.isKinematic = true;
        _index = 0;
        _animListExecuted = false;
        _animScriptCommands.Clear();
        _rendererPosDif.Clear();
        _inertTimer = 0;
    }
       
    public void SetAttackingState()
    {
        CurrentState = PlayerState.Attacking;
    }

    public IEnumerator UnsetAttackingState()
    {
        Debug.Log("unset");
        yield return new WaitForSecondsRealtime(0.5f);
        CurrentState = PlayerState.Grounded;
        Debug.Log("grounded");
    }

    public IEnumerator InertTimerZero()
    {
        yield return new WaitForSeconds(0.2f);
        _inertTimer = 0;
    }
    #endregion
}


