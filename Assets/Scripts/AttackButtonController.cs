using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Required when using Event data.
using Scripts.Variables;

public class AttackButtonController : MonoBehaviour, IPointerDownHandler // required interface when using the OnPointerDown method.
{
    public GameEvent AttackButtonDownEvent;
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + " Was Clicked.");
        AttackButtonDownEvent.Raise();

    }
}
