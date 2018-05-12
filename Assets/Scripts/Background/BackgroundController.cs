using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour {

    public GameObject Player;
    private Vector2 playerPos;
    public float scrollRate;
    private float diffX = 0f;
    private float diffY = 0f;
    private float differenceX;
    private float differenceY;
    // Use this for initialization
    void Start ()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerPos = new Vector3(Player.transform.position.x, Player.transform.position.y);
        //transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, transform.position.z);
        differenceX = gameObject.transform.position.x - Player.transform.position.x;
        differenceY = gameObject.transform.position.y - Player.transform.position.y;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Player.transform.position.x - playerPos.x > scrollRate * 3f)
        {
            diffX++;
            playerPos = new Vector3(Player.transform.position.x, Player.transform.position.y);
        }
        else if (Player.transform.position.x - playerPos.x < scrollRate * -3f)
        {
            diffX--;
            playerPos = new Vector3(Player.transform.position.x, Player.transform.position.y);
        }

        if (Player.transform.position.y > 17f)
        {
            if (Player.transform.position.y - playerPos.y > scrollRate * 3f)
            {
                diffY++;
                playerPos = new Vector3(Player.transform.position.x, Player.transform.position.y);
            }
            else if (Player.transform.position.y - playerPos.y < scrollRate * -3f)
            {
                diffY--;
                playerPos = new Vector3(Player.transform.position.x, Player.transform.position.y);
            }
            transform.position = new Vector3(differenceX + Player.transform.position.x - 0.01f * diffX, differenceY + Player.transform.position.y - 0.01f * diffY, transform.position.z);
        }
        else transform.position = new Vector3( differenceX + Player.transform.position.x - 0.01f*diffX, transform.position.y, transform.position.z);
    }

}
