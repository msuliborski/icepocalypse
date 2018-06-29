using UnityEngine;
using Scripts.Variables;

public class FoodController : MonoBehaviour
{
    private bool _canBeEaten = false;
    public GameEvent PlayerEatEvent;
	void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            PlayerEatEvent.Raise();
            Destroy(gameObject);
        } 
    }

    

}
