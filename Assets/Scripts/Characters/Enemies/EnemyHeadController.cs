using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeadController : MonoBehaviour {

    Rigidbody2D _rb;
    CircleCollider2D _col;

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            _rb.freezeRotation = true;
            //transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            _rb.velocity = new Vector3(0f, 0f, 0f);
            _rb.isKinematic = true;
            _col.isTrigger = true;
        }
    }


    // Use this for initialization
    void Start () {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CircleCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
