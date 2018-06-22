using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodController : MonoBehaviour
{
    public bool Eaten = false;
    private PlayerControllerExperimental _playerControllerScript;
    public Animator Animator;

    // Use this for initialization
    void Start ()
    {
        _playerControllerScript = GameObject.FindWithTag("Player").GetComponent<PlayerControllerExperimental>();
        Animator = GetComponent<Animator>();
    }
	void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player") _playerControllerScript.CurrentlyActiveFood = gameObject;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player") _playerControllerScript.CurrentlyActiveFood = null;
    }
	
}
