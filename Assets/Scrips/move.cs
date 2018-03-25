using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour {

    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
        //transform.Translate(new Vector2(3.0f*Time.deltaTime, 0.0f));
        //_rb.velocity = new Vector2(3.0f, _rb.velocity.y);
	}
}
