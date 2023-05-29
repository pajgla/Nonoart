using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovementController : MonoBehaviour
{
    [SerializeField] float m_MovementSensitivity = 1.0f;
    [SerializeField] RectTransform m_GridRectTransform = null;

    bool m_CanDrag = false;

    Vector3 m_LastMousePosition = Vector3.zero;

    public void Init(RectTransform gridRect)
    {
        m_GridRectTransform = gridRect;
    }

    public void SetCanDrag(bool canDrag)
    {
        m_CanDrag = canDrag;
    }

    private void Update()
    {
        if (m_GridRectTransform == null || m_CanDrag == false)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            m_LastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - m_LastMousePosition;
            m_GridRectTransform.Translate(delta *  m_MovementSensitivity * Time.deltaTime);
            m_LastMousePosition = Input.mousePosition;
        }
    }
}
