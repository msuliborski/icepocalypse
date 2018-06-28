using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dialogMessageString : MonoBehaviour {
	private GameObject Panel;
	public string msg;
	public int duration;
	public bool onTop;
	
	private bool readAlready = false;

	void Start(){
		Panel = GameObject.FindGameObjectWithTag("dialogPanel");
		if (!onTop) Panel = GameObject.FindGameObjectWithTag("dialogPanelBottom");
	}

	void OnTriggerEnter2D(Collider2D col){
        if (col.gameObject.tag == "Player" && !readAlready) {
            readAlready = true;
			Panel.GetComponentInChildren<DialogMessage>().DisplayMessage(msg, duration, true);
        }
    }

	void Update(){
	}
	
	
}
