using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeStopperDestroyer : MonoBehaviour
{
    public void TubeStopperDestroying()
    {
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }
	
}
