using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Required when using Event data.
using Scripts.Variables;

public class LeftArrowButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler // required interface when using the OnPointerDown method.
{
    
    public GameEvent LeftArrowButtonEnterEvent;
    public GameEvent LeftArrowButtonExitEvent;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + " Was Entered.");
        LeftArrowButtonEnterEvent.Raise();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeftArrowButtonExitEvent.Raise();
        Debug.Log(this.gameObject.name + " Was Exit.");
    }

}
