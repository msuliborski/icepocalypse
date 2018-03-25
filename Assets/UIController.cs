using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Variables;
using UnityEngine;
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
		Debug.Log("stop button clicked");
		var pce = Player.GetComponent<PlayerControllerExperimental>();
		pce.StopMovement();
		
		var leftButton = LeftButton.GetComponent<Button>();
		leftButton.interactable = true;

		var rightButton = RightButton.GetComponent<Button>();
		rightButton.interactable = true;

		var stopButton = StopButton.GetComponent<Button>();
		stopButton.interactable = false;
		
	}

	private void CheckGesture(float distance)
	{
		if (distance > SwipeDetectDistance)
		{
			Debug.Log("swipe up");
			var pce = Player.GetComponent<PlayerControllerExperimental>();
			pce.Jump();
		}
		else
		{
			Debug.Log("swipe down");
		}
		
		Debug.Log(distance);
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
				var distance = Input.mousePosition.y - _startPosition.y;
				CheckGesture(distance);
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
				var distance = touch.position.y - _startPosition.y;
				CheckGesture(distance);
				break;
				
;		}
		

	#endif
	}
}
