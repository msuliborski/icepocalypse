using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public float TriggerRange = 10.0f;
	public float PlayerSpeed = 5.0f;

    private GameObject _playerObject;
    private Rigidbody2D _rb;

	enum PlayerState 
	{
		Idle,
		Running,
		Attacking
	}

	private PlayerState _playerState = PlayerState.Idle;

	
	
	void Start ()
	{
        _playerObject = GameObject.FindGameObjectWithTag("Player");
	    _rb = GetComponent<Rigidbody2D>();
	}
	
	

	void Update () 
	{

        if ( transform.position.x - _playerObject.transform.position.x < TriggerRange && _playerState == PlayerState.Idle ) 
        {
	        _rb.velocity = new Vector2(-PlayerSpeed, _rb.velocity.y);
	        _playerState = PlayerState.Running;
	        Debug.Log("asdadasdasd");
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
	
	
	


}
