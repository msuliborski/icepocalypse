using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public float TriggerRange = 10.0f;

    private GameObject _playerObject;
    private bool _move = true;

	// Use this for initialization
	void Start () {
        _playerObject = GameObject.FindGameObjectWithTag("Player");
    }
	
	// Update is called once per frame
	void Update () {

        if( transform.position.x - _playerObject.transform.position.x < TriggerRange )
        {
            Debug.Log("TRIGGER");
        }

        if( _move )
        {
            transform.Translate(new Vector2(-3.0f * Time.deltaTime, 0.0f));
        }
    
	}

    void OnCollisionEnter2D( Collision2D col )
    {
        if( col.gameObject.tag == "Player" )
        {
            _move = false;
        }
    }

}
