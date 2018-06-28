using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;	
using UnityEngine;

public class DialogMessage : MonoBehaviour {

	Text textBox;
	Image imageBox;


	void Start() {
			
		textBox = GetComponentsInChildren<Text>()[0];
		imageBox = GetComponentsInChildren<Image>()[0];

		GetComponent<CanvasGroup>().alpha = 0.0f;
    }
	
	public void DisplayMessage(string msg, int duration){
		textBox.text = msg;
		StartCoroutine(dsp(msg, duration));
	}

	IEnumerator dsp(string msg, int duration){
		GetComponent<CanvasGroup>().alpha = 1.0f;
        yield return new WaitForSeconds(duration);	
		DismissMessage();
    }

	public void DismissMessage() {
		GetComponent<CanvasGroup>().alpha = 0.0f;
	}
	
}
