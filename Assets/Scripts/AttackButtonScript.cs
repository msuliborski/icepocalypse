using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButtonScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseDown()
    {
        if ( !GameObject.FindGameObjectWithTag("Player").GetComponent<FightSystem>().IsQTE )
        GameObject.FindGameObjectWithTag("Player").GetComponent<FightSystem>().ClickedAttack = true;
    }
}
