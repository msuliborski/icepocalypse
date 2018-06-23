using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;	
using UnityEngine;

public class DialogMessage : MonoBehaviour {

	Text textBox;
	Image imageBox;
	Button button;


	float timeScaleBackup;
	bool freezDisplay = false;


	void Start() {
			
		textBox = GetComponentsInChildren<Text>()[0];
		imageBox = GetComponentsInChildren<Image>()[0];
		button = GetComponentsInChildren<Button>()[0];
		

		DisplayMessage("chuj");
    }
	
	public void DisplayMessage(string msg){

		Time.timeScale = 0.0f;
		
		//imageBox.enabled = true;
		//textBox.enabled = true;
		//button.enabled = true;
		textBox.text = msg;
		GetComponent<CanvasGroup>().alpha = 1.0f;

	}

	public void DismissMessage() {
		Time.timeScale = 1.0f;
		//GetComponentsInChildren<Text>()[0].enabled = false;
		//GetComponentsInChildren<Image>()[0].enabled = false; 
		//GetComponentsInChildren<Button>()[0].enabled = false; 
		GetComponent<CanvasGroup>().alpha = 0.0f;
	}
	
}
