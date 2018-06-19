using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Scripts.Variables;
public class ExplosiveBarrelController : MonoBehaviour
{

    #region Variables
    public GameObject Explosion;
    public GameEvent BarrelExplosionEvent;
    #endregion

    #region Collisions

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            BarrelExplosionEvent.Raise();
            Instantiate(Explosion, collision.gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    #endregion
}
