using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrelController : MonoBehaviour {

    #region Variables
    private GameObject _player;
    private PlayerControllerExperimental _playerScript;
    public GameObject Explosion; 
    #endregion
    
    #region Collisions
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject == _player)
        {
            _playerScript.Hp -= 5;
            Instantiate(Explosion, col.gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    #endregion

    #region Start&Update
    // Use this for initialization
    void Start ()
    {
        _player = GameObject.Find("Player 1");
        _playerScript = _player.GetComponent<PlayerControllerExperimental>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    #endregion
}
