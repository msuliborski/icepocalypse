using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{

    #region Variables
    public GameObject Player;
    private Vector2 playerPos;
    public float scrollRate;
    private float differenceX;
    private float differenceY;
    #endregion

    #region Start&Update
    // Use this for initialization
    void Start ()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerPos = Player.transform.position;
        differenceX = gameObject.transform.position.x - Player.transform.position.x;
        differenceY = gameObject.transform.position.y - Player.transform.position.y;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        differenceX += -scrollRate * (Player.transform.position.x - playerPos.x);
        differenceY += -scrollRate * (Player.transform.position.y - playerPos.y);
        if (Player.transform.position.y > 16.65f) transform.position = new Vector3(Player.transform.position.x + differenceX, Player.transform.position.y + differenceY, transform.position.z);
        else transform.position = new Vector3(Player.transform.position.x + differenceX, transform.position.y, transform.position.z);
        playerPos = Player.transform.position;
    }
    #endregion
}
