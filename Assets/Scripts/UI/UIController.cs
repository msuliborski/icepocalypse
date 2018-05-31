using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Variables;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	public GameObject Player;
	public float SwipeDetectDistance = 100.0f;


	private Vector2 _startPosition;
	private bool _touching = false;
	
	public void OnLeft()
	{
		Debug.Log("facing left");
		var pce = Player.GetComponent<PlayerControllerExperimental>();
		pce.OnLeftDirection();

	}

	public void OnRight()
	{
		Debug.Log("facing right");
		var pce = Player.GetComponent<PlayerControllerExperimental>();
		pce.OnRightDirection();
		
	}

    public void OnTop() {
        var pce = Player.GetComponent<PlayerControllerExperimental>();
		pce.OnTopDirection();
    }

    public void OnDown() 
    {
        var pce = Player.GetComponent<PlayerControllerExperimental>();
		pce.OnDownDirection();
    }

	private void CheckGesture(Vector2 dist)
	{
		if (dist.x > SwipeDetectDistance * 2.5f)
		{
			OnRight();
		}
		else if (dist.x < -SwipeDetectDistance * 2.5f)
		{
			OnLeft();
        }


        if (dist.y > SwipeDetectDistance) 
        {
            OnTop();
        } 
        else if (dist.y < -SwipeDetectDistance) 
        {
            OnDown();
        }
	}


	private void Update()
	{
	#if UNITY_EDITOR 
		if (Input.GetButtonDown("Fire1"))
		{
			_touching = true;
			_startPosition = Input.mousePosition;
		}

		if (Input.GetButtonUp("Fire1"))
		{
			if (_touching)
			{
				Vector2 dist;
				dist.x = Input.mousePosition.x - _startPosition.x;
				dist.y = Input.mousePosition.y - _startPosition.y;
				CheckGesture(dist);
			}
			_touching = false;
		}
	#else
		if(Input.touchCount != 1) return;
		var touch = Input.touches.First();
		switch (touch.phase)
		{
			case TouchPhase.Began:
				_startPosition = touch.position;
				break;
			
			case TouchPhase.Ended:
				Vector2 dist;
				dist.x = Input.mousePosition.x - _startPosition.x;
				dist.y = Input.mousePosition.y - _startPosition.y;
				CheckGesture(dist);
				break;
        }
	#endif
	}
}
