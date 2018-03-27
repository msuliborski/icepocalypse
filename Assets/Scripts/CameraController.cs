using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject Player;
    private float _differenceX;
    private float _differenceY;
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        gameObject.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, gameObject.transform.position.z);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        gameObject.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, gameObject.transform.position.z);

    }
};



/*
public class CameraController : MonoBehaviour
{

    public GameObject Player;
    private float _differenceX;
    private float _differenceY;
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        _differenceX = gameObject.transform.position.x - Player.transform.position.x;
        _differenceY = gameObject.transform.position.y - Player.transform.position.y;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        gameObject.transform.position = new Vector3(_differenceX + Player.transform.position.x, _differenceY + Player.transform.position.y, gameObject.transform.position.z);

    }
};
*/
