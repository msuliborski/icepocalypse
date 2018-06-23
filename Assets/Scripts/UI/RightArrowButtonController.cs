using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Required when using Event data.
using Scripts.Variables;

public class RightArrowButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler // required interface when using the OnPointerDown method.
{
    public GameEvent RightArrowButtonEnterEvent;
    public GameEvent RightArrowButtonExitEvent;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + " Was Entered.");
        RightArrowButtonEnterEvent.Raise();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RightArrowButtonExitEvent.Raise();
        Debug.Log(this.gameObject.name + " Was Exit.");
    }

}
