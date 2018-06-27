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
            Debug.Log("Player enters food area!");
            _canBeEaten = true;
        } 
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player") 
        {
            Debug.Log("Player exits food area!");
            _canBeEaten = false;
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("food clicked!");
        if (_canBeEaten)
        {
            Debug.Log("food eaten!");
            PlayerEatEvent.Raise();
            Destroy(gameObject);
        }
    }

}
