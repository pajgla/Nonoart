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

    private void Start()
    {
        GridSpawner.OnGridSpawnedEvent += OnGridSpawnedEventCallback;
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

    private void OnGridSpawnedEventCallback(object caller, EventArgs args)
    {
        m_CanDrag = true;
    }
}
