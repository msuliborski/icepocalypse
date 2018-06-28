using UnityEngine;
using Scripts.Variables;
public class FinishLineController : MonoBehaviour {

    public GameEvent PlayerWin;
    private bool _crossed = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_crossed)
            return;
        
        if (collision.tag == "Player")
        {
            Debug.Log("player finishd map");
            PlayerWin.Raise();
            _crossed = true;
        }
    }
}
