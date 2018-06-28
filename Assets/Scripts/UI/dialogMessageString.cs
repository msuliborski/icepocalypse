using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dialogMessageString : MonoBehaviour {
	private GameObject Panel;
	public string msg;
	public int duration;
	
	private bool readAlready = false;

	void Start(){
		
		Panel = GameObject.FindGameObjectWithTag("dialogPanel");
		//Panel.GetComponentInChildren<DialogMessage>().DisplayMessage(msg, 3);
		Debug.Log("chuj");
	}

	void OnTriggerEnter2D(Collider2D col){
        if (col.gameObject.tag == "Player" && !readAlready) 
        {
            readAlready = true;
			Panel.GetComponentInChildren<DialogMessage>().DisplayMessage(msg, duration);
			Debug.Log("cwel");
        }
    }

	void Update(){
	}
	
	
}
