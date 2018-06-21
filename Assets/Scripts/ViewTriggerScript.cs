using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTriggerScript : MonoBehaviour {

    public GameObject FatherObject;

	// Use this for initialization
	//void Start () {
      //  _enemyObject = transform.parent.GetComponent<GameObject>();
	//}
	
	// Update is called once per frame
	//void Update () {
		
	//}

    void OnTriggerEnter2D( Collider2D col )
    {
        if (col.gameObject.tag == "Player")
        {
            FatherObject.GetComponent<EnemyController>().CatchPlayer();
        }
    }
}
