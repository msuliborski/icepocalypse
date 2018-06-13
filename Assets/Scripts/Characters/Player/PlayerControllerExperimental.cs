﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;


public class PlayerControllerExperimental : MonoBehaviour
{
    #region Variables
    public float JumpForce = 18;
    public float WallJumpForce = 24;
    public float WallReflectionForce = 17;
    public float MoveSpeed = 10;
    public int Hp = 100;
    private float _wallTimer = 0;
    public bool FacingRight = true;
    private float _moveDirection = 0;
    private bool _ignoreLedderEdge = false;
    private List<Vector3> _scriptDestinations;
    private float _scriptSpeed;
    public bool _isScripting = false;
    private bool _scriptSpeedAccelarated = false;
    private float _scriptAcceleration = 0;
    private int _index;
    private int _indexToRotate = -1;
    private Quaternion _scriptRotation;
    private int _indexToReact = -1;
    private KeyCode _keyToReact;
    private List<KeyCode> _keysToReact;
    private List<Vector3> _rendererPosDif;

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
        UpTrue,
<<<<<<< HEAD
        UpFalse,
        Up1True,
        Up1False,
        Up2True,
        Up2False,
        UpLadderTrue,
        UpLadderFalse
=======
        UpFalse
>>>>>>> 8f39dc3f66dbb51f91437d17eba9a407f908ceac
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
        _keysToReact = new List<KeyCode>();
        _animScriptCommands = new List<List<animScriptCommands>>();
        _rendererPosDif = new List<Vector3>();

        _onLeftDirection = false;
        _onRightDirection = false;
        _onTopDirection = false;
        _onDownDirection = false;
    }

    bool _onLeftDirection;
    bool _onRightDirection;
    bool _onTopDirection;
    bool _onDownDirection;

    public void OnLeftDirection() 
    {
        if (!_isScripting) _onLeftDirection = true;
    }

    public void OnRightDirection() 
    {
        if (!_isScripting) _onRightDirection = true;
    }

    public void OnTopDirection()
    {
        if (CurrentState != PlayerState.TubeSliding)
            _onTopDirection = true;
    }

    public void OnDownDirection()
    {
        if(
            !_isScripting || 
            (_isScripting && CurrentState == PlayerState.EdgeLaddering) ||
            (_isScripting && CurrentState == PlayerState.EgdeClimbingBody) ||
            (CurrentState == PlayerState.TubeSliding && 
            _isScripting && 
            _rigidbody.transform.position == _scriptDestinations[_index])
        )
        {
            _onDownDirection = true;
        }
             
    }

    void Update()
    {
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
                        _onTopDirection = false;
                        OnKeySpace();
                    } 
                    if (Physics2D.IsTouchingLayers(_colliderCorner, Wall)) _animator.SetBool("Movement", false);
                    else _animator.SetBool("Movement", (_moveDirection != 0) ? true : false);
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

                    if (Physics2D.IsTouchingLayers(_colliderLegs, Ground)) _animator.SetBool("LadderMovement", false);
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
<<<<<<< HEAD
=======
                   /* if (_rigidbody.velocity.x == 0.0f)
                    {
                        if (FacingRight) _rigidbody.velocity = new Vector2(0.0000001f, -0.0000001f);
                        else _rigidbody.velocity = new Vector2(-0.0000001f, -0.0000001f);
                    }*/
>>>>>>> 8f39dc3f66dbb51f91437d17eba9a407f908ceac
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
            switch (CurrentState)
            {
                case PlayerState.EdgeLaddering:
                    _isScripting = false;
                    _rigidbody.isKinematic = false;
                    if (FacingRight) _rigidbody.velocity = new Vector2(0.5f, -3f);
                    else _rigidbody.velocity = new Vector2(-0.5f, -3f);
                    _ignoreLedderEdge = true;
                    _animator.SetBool("LadderMovement", true);
                    break;

                case PlayerState.TubeSliding:
                    
                    _animator.SetBool("Tube Idle", false);
                    _animator.SetBool("Tube", false);
                    _animator.SetBool("Inert Down", true);

                   
                    _index++;
                    break;

                default:
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
            switch (CurrentState)
            {
                case PlayerState.EdgeLaddering:
                    _isScripting = false;
                    _rigidbody.isKinematic = false;
                    _rigidbody.velocity = new Vector2(3f, 0);
                    SetFacingRight(true);
                    _ignoreLedderEdge = true;
                    _animator.SetBool("Ladder", false);
                    break;

                default:
                    _index++;
                    break;
            }
        }
        else
        {
            switch (CurrentState)
            {
                case PlayerState.Laddering:
                    MoveRightOnLadder();
                    _animator.SetBool("LadderMovement", false);
                    _animator.SetBool("Ladder", false);
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
            switch (CurrentState)
            {
                case PlayerState.EdgeLaddering:
                    _isScripting = false;
                    _rigidbody.isKinematic = false;
                    _rigidbody.velocity = new Vector2(-3f, 0);
                    SetFacingRight(false);
                    _ignoreLedderEdge = true;
                    _animator.SetBool("Ladder", false);
                   break;

                default:
                    _index++;
                    break;
            }
        }
        else
        {
            switch (CurrentState)
            {
                case PlayerState.Laddering:
                    MoveLeftOnLadder();
                    _animator.SetBool("LadderMovement", false);
                    _animator.SetBool("Ladder", false);
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
            switch (CurrentState)
            {
                case PlayerState.EdgeLaddering:
                    _index++;
                    
                    _animator.SetBool("Ladder", false);
                    _animator.SetBool("UpLadder" , true);
                    break;
                default:
                    _index++;
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
        _renderer.transform.position = new Vector2(transform.position.x, transform.position.y);
        _wallTimer = 0;
        _rigidbody.gravityScale = 10;
    }

    private void onGroundImpact()
    {
        _ignoreLedderEdge = false;
        _animator.SetBool("Inert Down", false);
        _animator.SetBool("WallReflection", false);
        _animator.SetBool("JumpToWall", false);
        _animator.SetBool("Czekaning", false);
        _animator.SetBool("Slope", false);
        _renderer.transform.position = new Vector2(transform.position.x, transform.position.y);
        _wallTimer = 0;
        _rigidbody.gravityScale = 10;
    }

    private void onLadderImpact()
    {
        _animator.SetBool("Ladder", true);
        _rigidbody.gravityScale = 0;
        if (!_ignoreLedderEdge) _rigidbody.velocity = new Vector2(0f, 0f);
        else
        {
            _ignoreLedderEdge = false;
            _rigidbody.velocity = new Vector2(0f, -3f);
        }
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
    }


    private void onTubeImpact()
    {
        _scriptDestinations.Clear();
        _animator.SetBool("Ladder", false);
        _animator.SetBool("LadderMovement", false);
        _animator.SetBool("Tube", false);
        _animator.SetBool("Inert Down", false);
        _animator.SetBool("Movement", false);
        _animator.SetBool("WallReflection", false);
        _animator.SetBool("Czekaning", false);
        _animator.SetBool("Slope", false);
        GameObject go = findClosestObjectWithTag("tubeEnd", 1);
        if (FacingRight)
        {
            _scriptDestinations.Add(new Vector3(go.transform.position.x + playerWidth, transform.position.y, 0f));
            _scriptDestinations.Add(new Vector3(go.transform.position.x + playerWidth, go.transform.position.y + playerHeight / 4, 0f));
        }
        else
        {
            _scriptDestinations.Add(new Vector3(go.transform.position.x - playerWidth, transform.position.y, 0f));
            _scriptDestinations.Add(new Vector3(go.transform.position.x - playerWidth, go.transform.position.y + playerHeight / 4, 0f));
        }
        _scriptSpeed = 3.0f;
        _isScripting = true;
        _scriptSpeedAccelarated = false;
        _rigidbody.isKinematic = true;
        _rigidbody.velocity = new Vector3(0, 0, 0);
        _index = 0;
        _indexToReact = 1;
        _indexToRotate = -1;
        _keysToReact.Clear();
        _keysToReact.Add(KeyCode.DownArrow);
        _animScriptCommands.Clear();
        _animScriptCommands.Add(new List<animScriptCommands> { animScriptCommands.MovementFalse, animScriptCommands.TubeTrue });
        _animScriptCommands.Add(new List<animScriptCommands> { animScriptCommands.TubeIdleTrue });
        _rendererPosDif.Clear();
    }

    void onEdgeCornerImpact()
    {
        _scriptDestinations.Clear();
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
        _isScripting = true;
        _scriptSpeedAccelarated = false;
        _rigidbody.isKinematic = true;
        _rigidbody.velocity = new Vector3(0, 0, 0);
        _index = 0;
        _indexToReact = 0;
        _indexToRotate = -1;
        _keysToReact.Clear();
        _keysToReact.Add(KeyCode.Space);
        if (FacingRight) _keysToReact.Add(KeyCode.LeftArrow);
        else _keysToReact.Add(KeyCode.RightArrow);
        _animScriptCommands.Clear();
        _animScriptCommands.Add(new List<animScriptCommands> {animScriptCommands.CzekaningTrue});
        _animScriptCommands.Add(new List<animScriptCommands> { animScriptCommands.CzekaningFalse, animScriptCommands.MovementTrue});
        _rendererPosDif.Clear();
    }

    void onLadderEdgeImpact()
    {
        _animator.SetBool("LadderMovement", false);
        _scriptDestinations.Clear();
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
        _isScripting = true;
        _rigidbody.isKinematic = true;
        _scriptSpeedAccelarated = false;
        _rigidbody.velocity = new Vector3(0, 0, 0);
        _index = 0;
        _indexToReact = 0;
        _indexToRotate = -1;
        _keysToReact.Clear();
        _keysToReact.Add(KeyCode.Space);
        _keysToReact.Add(KeyCode.DownArrow);
        if (FacingRight)_keysToReact.Add(KeyCode.LeftArrow);
        else _keysToReact.Add(KeyCode.RightArrow);
        _animScriptCommands.Clear();
        _animScriptCommands.Add(new List<animScriptCommands> { animScriptCommands.MovementFalse});
        _animScriptCommands.Add(new List<animScriptCommands> { animScriptCommands.UpLadderFalse });
        _animScriptCommands.Add(new List<animScriptCommands> { animScriptCommands.MovementTrue });
        _rendererPosDif.Clear();
    }


    void onLadderEdge1Impact()
    {
        _scriptDestinations.Clear();
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
        _isScripting = true;
        _rigidbody.isKinematic = true;
        _scriptSpeedAccelarated = false;
        _rigidbody.velocity = new Vector3(0, 0, 0);
        _index = 0;
        _indexToReact = -1;
        _indexToRotate = -1;
        _keysToReact.Clear();
        _animScriptCommands.Clear();
        _animScriptCommands.Add(new List<animScriptCommands> { });
        _animScriptCommands.Add(new List<animScriptCommands> { animScriptCommands.LadderTrue });
        _rendererPosDif.Clear();
    }

    void onEdgeBodyImpact()
    {
        _animator.SetBool("WallReflection", false);
        _scriptDestinations.Clear();
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
        _isScripting = true;
        _scriptSpeedAccelarated = false;
        _rigidbody.isKinematic = true; 
        _rigidbody.velocity = new Vector3(0, 0, 0);
        _index = 0;
        _indexToReact = -1;
        _indexToRotate = -1;
        _animScriptCommands.Clear();
       _rendererPosDif.Clear();
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
        if (_scriptSpeedAccelarated) _scriptSpeed += _scriptAcceleration;
        transform.position = Vector2.MoveTowards(_rigidbody.transform.position, _scriptDestinations[_index], _scriptSpeed * Time.deltaTime);
        if (_index < _rendererPosDif.Count)
        {
            _renderer.transform.position = _rendererPosDif[_index] + transform.position;
        }
        if (_rigidbody.transform.position == _scriptDestinations[_index])
        {

            if (_index < _animScriptCommands.Count)
            {
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

                        case animScriptCommands.UpTrue:
                            _animator.SetBool("Up", true);
                            break;

                        case animScriptCommands.UpFalse:
                            _animator.SetBool("Up", false);
                            break;
<<<<<<< HEAD

                        case animScriptCommands.Up1True:
                            _animator.SetBool("Up1", true);
                            break;

                        case animScriptCommands.Up1False:
                            _animator.SetBool("Up1", false);
                            break;

                        case animScriptCommands.Up2True:
                            _animator.SetBool("Up2", true);
                            break;

                        case animScriptCommands.Up2False:
                            _animator.SetBool("Up2", false);
                            break;

                        case animScriptCommands.UpLadderTrue:
                            _animator.SetBool("UpLadder", true);
                            break;

                        case animScriptCommands.UpLadderFalse:
                            _animator.SetBool("UpLadder", false);
                            break;
=======
>>>>>>> 8f39dc3f66dbb51f91437d17eba9a407f908ceac
                    }
                }
            }

            if (_indexToReact == _index)
            {
                if (_onTopDirection) {
                    OnKeySpace();
                    _onTopDirection = false;
                }

                if (_onDownDirection)
                {
                    OnKeyDown();
                    _onDownDirection = false;
                } 

                if (_onRightDirection) {
                    OnKeyRight();
                    _onRightDirection = false;
                }

                if (_onLeftDirection) {
                    OnKeyLeft();
                    _onLeftDirection = false;
                }

                for (int i = 0; i < _keysToReact.Count; i++)
                {
                    if (Input.GetKeyDown(_keysToReact[i]))
                    {
                        switch (_keysToReact[i])
                        {
                            case KeyCode.Space:
                                OnKeySpace();
                                break;
                            case KeyCode.DownArrow:
                                OnKeyDown();
                                break;
                            case KeyCode.LeftArrow:
                                OnKeyLeft();
                                break;
                            case KeyCode.RightArrow:
                                OnKeyRight();
                                break;
                        }
                    }
                }
            }
            else
            {
                if (_index == _indexToRotate) transform.rotation = _scriptRotation;
                
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
    #endregion
}


