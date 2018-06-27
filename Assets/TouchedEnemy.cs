using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchedEnemy : MonoBehaviour {

    private GameObject _playerObject;
    private GameObject _enemyObject;
    public GameObject DickTrigger;

	// Use this for initialization
	void Start () {
        _playerObject = GameObject.FindGameObjectWithTag("Player");
        _enemyObject = transform.parent.gameObject;
        DickTrigger.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator TurnDickOff()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        DickTrigger.SetActive(false);
    }

    void OnTriggerEnter2D( Collider2D col )
    {
        if( col.gameObject.tag == "legs_collider" && _playerObject.GetComponent<Rigidbody2D>().velocity.x == 0f )
        {
            _playerObject.GetComponent<FightSystem>().KeepFalling();
            _enemyObject.GetComponent<EnemyController>().TurnOffYourPhysics();
            DickTrigger.SetActive(true);
            //StartCoroutine(TurnDickOff());
            //tube finiszer
        }
    }
}
