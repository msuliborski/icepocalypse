using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Scripts.Variables;

public class StartPanelController : MonoBehaviour {

    public GameEvent GameStartEvent;

    public void OnGameStartButton()
    {
        GameStartEvent.Raise();
    }
}
