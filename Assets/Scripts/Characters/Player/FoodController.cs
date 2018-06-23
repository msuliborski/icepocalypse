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
            _canBeEaten = true;
        } 
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player") 
        {
            _canBeEaten = false;
        }
    }

    private void OnMouseDown()
    {
        if (_canBeEaten)
        {
            PlayerEatEvent.Raise();
            Destroy(gameObject);
        }
    }

}
