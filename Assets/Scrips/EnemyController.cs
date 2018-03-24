using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public float PlayerSpeed = 5.0f;
    public Transform ViewRangeTransform;
    public Transform PatrolDistanceTransform;

    private GameObject _playerObject;
    private Rigidbody2D _rb;
    private float _patrolDistance;
    private float _patrolRangeA;
    private float _patrolRangeB;
    private float _triggerRange;

    enum Facing
    {
        Right,
        Left
    }

    private Facing _facing;

	enum PlayerState 
	{
		Idle,
		Running,
		Attacking
	}

	private PlayerState _playerState = PlayerState.Running;

	
	void Start ()
	{
        _playerObject = GameObject.FindGameObjectWithTag("Player");
	    _rb = GetComponent<Rigidbody2D>();
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
        if ( _playerState == PlayerState.Running )
        {
            if ( _facing == Facing.Left )
            {
                _rb.velocity = new Vector2(-PlayerSpeed, _rb.velocity.y);
                //Debug.Log("Running right");
            }
            else
            {
                _rb.velocity = new Vector2(PlayerSpeed, _rb.velocity.y);
                //Debug.Log("Running left");
            }
        }

        if ( _facing == Facing.Left )
        {
            if (  _playerObject.transform.position.x < transform.position.x && transform.position.x - _playerObject.transform.position.x < _triggerRange )
            {
                _playerState = PlayerState.Attacking;
                Debug.Log("Attacking left");
            }
        }
        else if (_playerObject.transform.position.x > transform.position.x && transform.position.x - _playerObject.transform.position.x < _triggerRange)
        {
            _playerState = PlayerState.Attacking;
            Debug.Log("Attacking right");
        }

        if(  _playerState != PlayerState.Attacking && ( transform.position.x <= _patrolRangeA && _facing == Facing.Left ) || ( transform.position.x >= _patrolRangeB && _facing == Facing.Right ) )
        {
            ChangeFacingDirection();
        }
       
	}

    void OnCollisionEnter2D( Collision2D col )
    {
        if ( col.gameObject.tag == "Player" )
        {
	        _rb.velocity = new Vector2(0, _rb.velocity.y);
	        _playerState = PlayerState.Attacking;
        }
    }

    void OnTriggerEnter2D( Collider2D col )
    {
        if (col.gameObject.tag == "Wall")
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
	
	
	


}
