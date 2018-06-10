using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour {

    Scene currentScene;
	
    // Use this for initialization
	void Start ()
    {
        currentScene = SceneManager.GetActiveScene();
    }
	
	// Update is called once per frame
	void Update ()
    {
        currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ldr")
        {
            if (transform.position.x > 167) SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }
}
