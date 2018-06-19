using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Variables;

public class DeathPanelController : MonoBehaviour {
    public GameEvent GameRestartEvent;

	public void OnGameRestart()
    {
        GameRestartEvent.Raise();
    }
}
