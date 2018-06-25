using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemysHeadScript : MonoBehaviour {

    public GameObject Head;
    public GameObject Corpse;

	void SpawnHead()
    {
        Instantiate(Head, new Vector3(transform.position.x + 0.08f, transform.position.y + 0.43f, transform.position.z), Quaternion.identity);
        Instantiate(Corpse, new Vector3(transform.position.x, transform.position.y - 0.8f, transform.position.z), Quaternion.identity);
    }
}
