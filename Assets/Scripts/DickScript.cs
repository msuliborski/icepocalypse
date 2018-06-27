using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DickScript : MonoBehaviour {

    private GameObject _playerObject;
    private GameObject _enemyObject;

    // Use this for initialization
    void Start()
    {
        _playerObject = GameObject.FindGameObjectWithTag("Player");
        _enemyObject = transform.parent.gameObject;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "legs_collider")
        {
            _playerObject.GetComponent<FightSystem>().FinisherFromAir();
            _enemyObject.GetComponent<EnemyController>().DestroyYourself();
            //tube finiszer
        }
    }
}
