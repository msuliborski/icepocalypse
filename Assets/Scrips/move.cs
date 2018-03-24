using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        transform.Translate(new Vector2(3.0f*Time.deltaTime, 0.0f));
	}
}
