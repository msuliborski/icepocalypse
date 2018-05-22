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
	public GameObject LeftButton;
	public GameObject RightButton;
	public GameObject StopButton;
	public float SwipeDetectDistance = 100.0f;


	private Vector2 _startPosition;
	private bool _touching = false;
	
	public void OnLeftButtonClicked()
	{
		Debug.Log("facing left");
		var pce = Player.GetComponent<PlayerControllerExperimental>();
		pce.SetFacingRight(false);

		var leftButton = LeftButton.GetComponent<Button>();
		leftButton.interactable = false;

		var rightButton = RightButton.GetComponent<Button>();
		rightButton.interactable = true;
		
		var stopButton = StopButton.GetComponent<Button>();
		stopButton.interactable = true;
		

	}

	public void OnRightButtonClicked()
	{
		Debug.Log("facing right");
		var pce = Player.GetComponent<PlayerControllerExperimental>();
		pce.SetFacingRight(true);
		
		
		var leftButton = LeftButton.GetComponent<Button>();
		leftButton.interactable = true;

		var rightButton = RightButton.GetComponent<Button>();
		rightButton.interactable = false;
		
		var stopButton = StopButton.GetComponent<Button>();
		stopButton.interactable = true;
	}

	public void OnStopButtonClicked()
	{
		SceneManager.LoadScene(0);
		
	}

	private void CheckGesture(Vector2 dist)
	{
		if (dist.y > SwipeDetectDistance)
		{
			Debug.Log("swipe up");
			var pce = Player.GetComponent<PlayerControllerExperimental>();
			pce.OnKeySpace();
		}
		else if(dist.y < -SwipeDetectDistance)
		{
			var pce = Player.GetComponent<PlayerControllerExperimental>();
			pce.OnKeyDown();
			Debug.Log("swipe down");
		}

		if (dist.x > SwipeDetectDistance * 2.5f)
		{
			OnRightButtonClicked();
		}
		else if (dist.x < -SwipeDetectDistance * 2.5f)
		{
			OnLeftButtonClicked();
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
				
;		}
		

	#endif
	}
}
