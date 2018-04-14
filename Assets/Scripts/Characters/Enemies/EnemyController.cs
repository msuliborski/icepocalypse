﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

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
        }
        else
        {
            _facing = Facing.Left;
        }

        _triggerRange = Mathf.Abs( ViewRangeTransform.position.x - transform.position.x );
	}
	
	

	void FixedUpdate () 
	{
        _playerEnemyDistance = _playerObject.transform.position.x - transform.position.x;
        GetState();

        if(  _playerState != PlayerState.Running && ( transform.position.x <= _patrolRangeA && _facing == Facing.Left ) || ( transform.position.x >= _patrolRangeB && _facing == Facing.Right ) )
        {
            ChangeFacingDirection();
        }
       
	}

    void OnCollisionEnter2D( Collision2D col )
    {
        if ( col.gameObject.tag == "Player" )
        {
	        _rb.velocity = new Vector2(0, _rb.velocity.y);
            Debug.Log("hit player");
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            //_playerState = PlayerState.Attacking;
        }
    }

    void OnTriggerEnter2D( Collider2D col )
    {
        if (col.gameObject.tag == "Wall" )
        {
            ChangeFacingDirection();
        }
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
                            Debug.Log("Running");
                        }
                    }
                    else if (_playerEnemyDistance > 0 )
                    {
                        _playerState = PlayerState.Running;
                        Debug.Log("Running");
                    }
                }

                break;

            case PlayerState.Running:
                Run(EnemyRunningSpeed);

                if (Mathf.Abs(_playerEnemyDistance) <= FightingDeadZone)
                {
                    _playerState = PlayerState.Attacking;
                    Debug.Log("Attacking , distance: " + Mathf.Abs(_playerEnemyDistance));
                }

                break;

            case PlayerState.Attacking:
                Attack();
                if (Mathf.Abs(_playerEnemyDistance) > FightingDeadZone)
                {
                    _playerState = PlayerState.Running;
                    Debug.Log("Running");
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
            //Debug.Log("Running left");
        }
        else if (_playerEnemyDistance<0)
        {
            _rb.velocity = new Vector2(-speed, _rb.velocity.y);
            //Debug.Log("Running left");
        }
    }

    void Attack()
    {
        _rb.velocity = new Vector2(0, _rb.velocity.y);
        //Debug.Log("Attacking");
    }

}