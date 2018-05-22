﻿﻿using System.Collections;
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
    private bool _ladderImpact = false;

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
        Ladder = LayerMask.GetMask("Ladder");
        LadderEdge = LayerMask.GetMask("LadderEdge");
        LadderEdge1 = LayerMask.GetMask("LadderEdge1");
        playerWidth = 1f;
        playerHeight = 2f;
        _scriptDestinations = new List<Vector3>();
        _keysToReact = new List<KeyCode>();
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
                    if (CurrentState != PlayerState.EgdeClimbingBody) PreviousState = CurrentState;
                    CurrentState = PlayerState.EgdeClimbingBody;
                }
                else if (Physics2D.IsTouchingLayers(_colliderCorner, Edge))
                {
                    if (CurrentState != PlayerState.EgdeClimbingCorner) PreviousState = CurrentState;
                    CurrentState = PlayerState.EgdeClimbingCorner;
                }
                else if ((Physics2D.IsTouchingLayers(_colliderCorner, LadderEdge) || Physics2D.IsTouchingLayers(_colliderBody, LadderEdge)) && !_ignoreLedderEdge)
                {
                    if (CurrentState != PlayerState.EdgeLaddering) PreviousState = CurrentState;
                    CurrentState = PlayerState.EdgeLaddering;
                }
                else if (Physics2D.IsTouchingLayers(_colliderLegs, LadderEdge1))
                {
                    if (CurrentState != PlayerState.EdgeLaddering1) PreviousState = CurrentState;
                    CurrentState = PlayerState.EdgeLaddering1;
                }
                else if (Physics2D.IsTouchingLayers(_colliderBody, Ladder))
                {
                    if (CurrentState != PlayerState.Laddering) PreviousState = CurrentState;
                    CurrentState = PlayerState.Laddering;
                }
                else if (Physics2D.IsTouchingLayers(_colliderLegs, Ground))
                {
                    if (CurrentState != PlayerState.Grounded) PreviousState = CurrentState;
                    CurrentState = PlayerState.Grounded;
                }
                else if (Physics2D.IsTouchingLayers(_colliderWhole, Tube))
                {
                    if (CurrentState != PlayerState.TubeSliding) PreviousState = CurrentState;
                    CurrentState = PlayerState.TubeSliding;
                }
                else if (Physics2D.IsTouchingLayers(_colliderBody, Wall))
                {
                    if (CurrentState != PlayerState.WallHugging) PreviousState = CurrentState;
                    CurrentState = PlayerState.WallHugging;
                }
                else if (Physics2D.IsTouchingLayers(_colliderWhole, HandBar))
                {
                    if (CurrentState != PlayerState.HandBarring) PreviousState = CurrentState;
                    CurrentState = PlayerState.HandBarring;
                }
                else if (Physics2D.IsTouchingLayers(_colliderWhole, Slope))
                {
                    if (CurrentState != PlayerState.Sloping) PreviousState = CurrentState;
                    CurrentState = PlayerState.Sloping;
                }
                else
                {
                    if (CurrentState != PlayerState.Inert) PreviousState = CurrentState;
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

                    if (Input.GetKeyDown(KeyCode.RightArrow)) OnKeyRight();
                    if (Input.GetKeyDown(KeyCode.LeftArrow)) OnKeyLeft();
                    if (Input.GetKeyDown(KeyCode.DownArrow)) OnKeyDown();
                    if (Input.GetKeyDown(KeyCode.Space)) OnKeySpace();

                    resetImpacts();
                    _ignoreLedderEdge = false;

                    break;

                case PlayerState.Inert:

                    resetImpacts();
                     
                    break;

                case PlayerState.WallHugging:

                    onWallImpact();
                    manageWallTimer();

                    if ((Input.GetKeyDown(KeyCode.Space))) OnKeySpace();

                    break;

                case PlayerState.Laddering:
                     
                    onLadderImpact();
                    if (Input.GetKeyDown(KeyCode.Space)) OnKeySpace();
                    if (Input.GetKeyDown(KeyCode.DownArrow)) OnKeyDown();
                    if (Input.GetKeyDown(KeyCode.RightArrow) && !FacingRight) OnKeyRight();
                    if (Input.GetKeyDown(KeyCode.LeftArrow) && FacingRight) OnKeyLeft();

                    break;
                
                case PlayerState.Sloping:

                    onSlopeImpact();

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


                case PlayerState.EdgeLaddering:

                    onLadderEdgeImpact();

                    break;

                case PlayerState.EdgeLaddering1:

                    onLadderEdge1Impact();

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
                    _ladderImpact = false;
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
                    _ladderImpact = false;
                    break;

                case PlayerState.EgdeClimbingCorner:
                    _isScripting = false;
                    _rigidbody.isKinematic = false;
                    _rigidbody.velocity = new Vector2(-_moveDirection * WallReflectionForce, JumpForce);
                    SetFacingRight(true);
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
                    _ladderImpact = false;
                    break;

                case PlayerState.EgdeClimbingCorner:
                    _isScripting = false;
                    _rigidbody.isKinematic = false;
                    _rigidbody.velocity = new Vector2(-_moveDirection * WallReflectionForce, JumpForce);
                    SetFacingRight(false);
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
                    break;
                default:
                    SetFacingRight(false);
                    break;
            }
        }
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
                if (FacingRight) SetFacingRight(false);
                else SetFacingRight(true);
            }
            else if (CurrentState == PlayerState.Laddering)
            {
                MoveUpOnLadder();
            }
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
    }

    public void MoveUpOnLadder()
    {
        _rigidbody.velocity = new Vector2(0f, 3f);
    }

    
    #endregion

    #region Impacts


    private void onLadderImpact()
    {
        if (!_ladderImpact)
        {
            _ladderImpact = true;
            _rigidbody.gravityScale = 0;
            if (!_ignoreLedderEdge) _rigidbody.velocity = new Vector2(0f, 0f);
            else
            {
                _ignoreLedderEdge = false;
                _rigidbody.velocity = new Vector2(0f, -3f);
            }
        }
    }

    private void onSlopeImpact()
    {
        _scriptDestinations.Clear();
        GameObject go = findClosestObjectWithTag("slopeEnd", 5);
        _scriptDestinations.Add(new Vector3(go.transform.position.x + go.GetComponent<SpriteRenderer>().bounds.size.x/2 + playerWidth/2 , go.transform.position.y - go.GetComponent<SpriteRenderer>().bounds.size.y / 2 + playerHeight/2));
        _scriptDestinations.Add(_scriptDestinations[0] + new Vector3(2f, 0f));
        if (go.transform.localScale.x > 0) SetFacingRight(true);
        else SetFacingRight(false);
        _scriptSpeed = 10.0f;
        transform.rotation = Quaternion.Euler(0, 0, -30);
        _isScripting = true;
        _scriptSpeedAccelarated = true;
        _scriptAcceleration = 0.2f;
        _rigidbody.isKinematic = true;
        _rigidbody.velocity = new Vector3(0, 0, 0);
        _index = 0;
        _indexToReact = -1;
        _indexToRotate = 0;
        _scriptRotation = Quaternion.Euler(0, 0, 0);
        _keysToReact.Clear();
    }


    private void onTubeImpact()
    {
        _scriptDestinations.Clear();
        GameObject go = findClosestObjectWithTag("tubeEnd", 1);
        _scriptDestinations.Add(new Vector3(go.transform.position.x + playerWidth, transform.position.y, 0f));
        _scriptDestinations.Add(new Vector3(go.transform.position.x + playerWidth, go.transform.position.y + playerHeight/4, 0f));
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
    }

    void onEdgeCornerImpact()
    {
        _scriptDestinations.Clear();
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
    }

    void onLadderEdgeImpact()
    {
        _scriptDestinations.Clear();
        GameObject go = findClosestObjectWithTag("ladderEdge", 1);
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
    }


    void onLadderEdge1Impact()
    {
        _scriptDestinations.Clear();
        GameObject go = findClosestObjectWithTag("ladderEdge", 1);
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
    }

    void onEdgeBodyImpact()
    {
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
    }

    private void onWallImpact()
    {
        if (!_wallImpact)
        {
            _wallImpact = true;
            if (PreviousState == PlayerState.Grounded) _rigidbody.velocity = new Vector2(0, 6); 
            else _rigidbody.velocity = new Vector2(0, 0);
            _rigidbody.gravityScale = 0;
        }
    }

    private void resetImpacts()
    {
        _wallTimer = 0;
        _rigidbody.gravityScale = 10;
        _wallImpact = false;
        _ladderImpact = false;
        
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
        if (_scriptSpeedAccelarated) _scriptSpeed += _scriptAcceleration;
        transform.position = Vector2.MoveTowards(_rigidbody.transform.position, _scriptDestinations[_index], _scriptSpeed * Time.deltaTime);
        if (_rigidbody.transform.position == _scriptDestinations[_index])
        {
            if (_indexToReact == _index)
            {
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


