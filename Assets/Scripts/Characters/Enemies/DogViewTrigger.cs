using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogViewTrigger : MonoBehaviour {

    public GameObject FatherObject;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            FatherObject.GetComponent<WildDogScript>().CatchPlayer();
        }
    }

}
