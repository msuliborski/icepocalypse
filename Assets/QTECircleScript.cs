using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTECircleScript : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Debug.Log("uwaga podaje pozycje x: " + transform.position.x + " y:" + transform.position.y + " z: " + transform.position.z);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnMouseDown()
    {
        Debug.Log("FINISHER!!!!!!");
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        GameObject.Find("Enemy 1").GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        GameObject.Find("Enemy 1").GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 100.0f));
        GameObject.Find("Enemy 1").transform.Rotate(new Vector3(0f, 0f, 100.0f));
    }
}
