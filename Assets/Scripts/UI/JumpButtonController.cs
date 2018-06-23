using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Required when using Event data.
using Scripts.Variables;

public class JumpButtonController : MonoBehaviour, IPointerDownHandler // required interface when using the OnPointerDown method.
{
    public GameEvent JumpButtonDownEvent;
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + " Was Clicked.");
        JumpButtonDownEvent.Raise();

    }
}
