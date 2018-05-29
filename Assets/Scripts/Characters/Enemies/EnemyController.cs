﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    public GameObject QteButton;

    public int EnemyHealthPoints = 100;
    private int _enemyHealthPoints;

    public bool _isUnderAttack = false;
    public bool _isDefending = false;

    private float _animatingTime = 0f;

    public float EnemyRunningSpeed = 5.0f;
    public float EnemyWalkingSpeed = 2.5f;
    public float FightingDeadZone = 0.5f;
    public Transform ViewRangeTransform;
    public Transform PatrolDistanceTransform;

    private GameObject _playerObject;

    private Rigidbody2D _rb;

    private float _patrolDistance;
    private float _patrolRangeA;
    private float _patrolRangeB;
    private float _triggerRange;

    private float _playerEnemyDistance;

    private Collider2D trig;

    private Animator _anim;

    private bool _isGrounded;

    private bool _isHurt;
    private GameObject _qteCircle = null;

    public float[] QTECirclePositionsArray;
    public float QTECircleInterval;
    private int _qteCircleIterator = 0;
    private float _timeStamp;

    enum Facing
    {
        Right,
        Left
    }

    private Facing _facing;

	enum PlayerState 
	{
		Idle,
        Walking,
		Running,
		Attacking
	}

	private PlayerState _playerState = PlayerState.Walking;

	
	void Start ()
	{

        _timeStamp = 0f;

        _isHurt = false;

       _anim = GetComponent<Animator>();
        _enemyHealthPoints = EnemyHealthPoints;

        //trig = GetComponentInChildren<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _playerObject = GameObject.FindGameObjectWithTag("Player");
        _patrolDistance = Mathf.Abs(transform.position.x - PatrolDistanceTransform.position.x);
        _patrolRangeA = transform.position.x - _patrolDistance;
        _patrolRangeB = transform.position.x + _patrolDistance;

        Debug.Log("A: " + _patrolRangeA + " B: " + _patrolRangeB);

        if (ViewRangeTransform.position.x > transform.position.x )
        {
            _facing = Facing.Right;
            //ChangeFacingDirection();
        }
        else
        {
            _facing = Facing.Left;
        }

        _triggerRange = Mathf.Abs( ViewRangeTransform.position.x - transform.position.x );

	}

    public void SetQTETimeStamp()
    {
        _timeStamp = Time.time;
    }

	void Update () 
	{
        if (_isHurt)
        {
            //Time.timeScale = 0.3f;

            if (_qteCircle == null)
            {
                //Debug.Log("czas: "+ (Time.time - _timeStamp));
                if ( Time.time - _timeStamp < QTECircleInterval )
                {
                    return;
                }

                Debug.Log("nowe kolko");

                if (_qteCircleIterator < QTECirclePositionsArray.Length)
                {
                    _qteCircle = PutCircle(new Vector3(transform.position.x, transform.position.y + QTECirclePositionsArray[_qteCircleIterator], transform.position.z));
                    _qteCircleIterator++;
                }
                else
                {
                    Debug.Log("osiagnieto max kolek.");
                }
            }

            return;

        }

        if ( Input.GetKey(KeyCode.Space) )
        {
            //_anim.SetBool("attack", true);
        }
        else
        {
            //_anim.SetBool("attack", false);
        }

        _playerEnemyDistance = _playerObject.transform.position.x - transform.position.x;

        GetState();

        if(  _playerState == PlayerState.Walking && ( transform.position.x <= _patrolRangeA && _facing == Facing.Left ) ||  _playerState == PlayerState.Walking && ( transform.position.x >= _patrolRangeB && _facing == Facing.Right ) )
        {
            ChangeFacingDirection();
        }
       
	}

    void OnCollisionEnter2D( Collision2D col )
    {
        if ( col.gameObject.tag == "Player" )
        {
	        _rb.velocity = new Vector2(0, _rb.velocity.y);
        }
    }

    void OnTriggerEnter2D( Collider2D col )
    {
        if (col.gameObject.tag == "Wall" )
        {
            ChangeFacingDirection();
        }

        if (col.gameObject.tag == "PlayerFist" && _isUnderAttack )//&& _isUnderAttack && !_isDefending )
        {
            Debug.Log("enemy health: " + _enemyHealthPoints);
            SetHealth(-10);
            _isUnderAttack = false;

            if (_enemyHealthPoints <= 0)
            {
                Debug.Log("smierc przeciwnika");
                _playerObject.GetComponent<FightSystem>().IsFighting = false;
                gameObject.SetActive(false);
                _enemyHealthPoints = EnemyHealthPoints;
                Time.timeScale = 1.0f;
            }

            if ( _enemyHealthPoints <= 20 && !_isHurt)
            {
                _isHurt = true;
                _playerObject.GetComponent<FightSystem>().IsQTE = true;
                _qteCircle = PutCircle(new Vector3(transform.position.x, transform.position.y + QTECirclePositionsArray[_qteCircleIterator], transform.position.z));
                _qteCircleIterator++;
                _timeStamp = Time.time;
            }
        }
    }

    void SetHealth(int value)
    {
        _enemyHealthPoints += value;
    }

    void ChangeFacingDirection()
    {
        if (_facing == Facing.Right)
        {
            _facing = Facing.Left;
        }
        else
        {
            _facing = Facing.Right;
        }

        transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void GetState()
    {
        switch( _playerState )
        {
            case PlayerState.Walking:
                Walk(EnemyWalkingSpeed);

                if (Mathf.Abs(_playerEnemyDistance) < _triggerRange)
                {
                    if (_facing == Facing.Left)
                    {
                        if (_playerEnemyDistance < 0 )
                        {
                            _playerState = PlayerState.Running;
                            //Debug.Log("Running");
                        }
                    }
                    else if (_playerEnemyDistance > 0 )
                    {
                        _playerState = PlayerState.Running;
                        //Debug.Log("Running");
                    }
                }

                break;

            case PlayerState.Running:
                Run(EnemyRunningSpeed);

                if (Mathf.Abs(_playerEnemyDistance) <= FightingDeadZone)
                {
                    _playerState = PlayerState.Attacking;
                    ProceedToFight();
                    //Debug.Log("Attacking , distance: " + Mathf.Abs(_playerEnemyDistance));
                }

                break;

            case PlayerState.Attacking:
                Attack();
                if (Mathf.Abs(_playerEnemyDistance) > FightingDeadZone)
                {
                    _playerState = PlayerState.Running;
                    _rb.constraints = RigidbodyConstraints2D.None;
                    _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                    Time.timeScale = 1.0f;
                    //Debug.Log("Running");
                }
                break;
        }
    }

    void Walk( float speed )
    {
        int _directionSpecifier = 1;

        if (_facing == Facing.Left)
        {
            _directionSpecifier *= -1;
        }

        _rb.velocity = new Vector2(speed*_directionSpecifier, _rb.velocity.y);
    }

    void Run(float speed)
    {
        if (_playerEnemyDistance>0)
        {
            _rb.velocity = new Vector2(speed, _rb.velocity.y);
            if ( _facing == Facing.Left )
            {
                ChangeFacingDirection();
            }
            //Debug.Log("Running left");
        }
        else if (_playerEnemyDistance<0)
        {
            _rb.velocity = new Vector2(-speed, _rb.velocity.y);
            if ( _facing == Facing.Right )
            {
                ChangeFacingDirection();
            }
            //Debug.Log("Running left");
        }
    }

    void ProceedToFight()
    {
        //Time.timeScale = 0.3f;
        GameObject.Find("Player 1").GetComponent<FightSystem>().Enemy = gameObject;
        GameObject.Find("Player 1").GetComponent<FightSystem>().IsFighting = true;
        //GameObject.Find("Player 1").GetComponent<FightSystem>().ProceedToFight();
        _rb.velocity = new Vector2(0, _rb.velocity.y);
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }

    GameObject PutCircle( Vector3 vector )
    {
        GameObject circle = Instantiate(QteButton, vector, Quaternion.identity) as GameObject;
        circle.GetComponent<QTECircleScript>().FatherObject = gameObject;
        //circle.transform.parent = gameObject.transform;
        return circle;
    }

    public void CancelQTE()
    {
        _isHurt = false;
        _playerObject.GetComponent<FightSystem>().IsQTE = false;
        //_qteCircleIterator = 0;
    }

    void Attack()
    {
               
        if ( _isDefending && (Time.time - _animatingTime) >= 1.333f )
        {
            _isDefending = false;
            //Debug.Log("wylaczam obrone");
        }

        float x = Random.Range(0f, 3.0f);

        if ( x < 1.0f )
        {
            _anim.SetBool("attack", true);
        }
        else if ( x >= 1.0f && x < 2.0f )
        {
            //
        }
        
    }

    public void Defend()
    {
        
        _isUnderAttack = true;
        float x = Random.Range(0f, 2.0f);

        if ( x <= 1.0f )
        {
            _anim.SetBool("defend", true);
            _animatingTime = Time.time;
            _isDefending = false;
            //Debug.Log("defend");
        }
        
    }

}
