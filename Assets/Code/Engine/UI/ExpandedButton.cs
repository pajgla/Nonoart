using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("UI/Expanded Button")]
public class ExpandedButton : Button
{
    UnityEvent e_OnMouseDown = new UnityEvent();
    UnityEvent e_OnMouseUp = new UnityEvent();

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        e_OnMouseDown.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        e_OnMouseUp.Invoke();
    }

    public UnityEvent OnMouseDown
    {
        get { return e_OnMouseDown; }
    }

    public UnityEvent OnMouseUp
    {
        get { return e_OnMouseUp; }
    }
}
