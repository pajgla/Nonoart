using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovementController : MonoBehaviour
{
    [SerializeField] float m_MovementSensitivity = 1.0f;
    [SerializeField] RectTransform m_GridRectTransform = null;

    bool m_CanDrag = false;
    bool m_IsTileRightClicked = false;

    Vector3 m_LastMousePosition = Vector3.zero;

    public void Init(RectTransform gridRect)
    {
        m_GridRectTransform = gridRect;

        GameManager.Get().GetGlobalEvents().e_OnTileClicked += OnTileClicked;
    }

    public void SetCanDrag(bool canDrag)
    {
        m_CanDrag = canDrag;
    }

    private void OnTileClicked(GridTile tile, KeyCode keyCode)
    {
        // Right click
        if (keyCode == KeyCode.Mouse1)
        {
            m_CanDrag = false;
            m_IsTileRightClicked = true;
        }
    }

    private void Update()
    {
        if (m_GridRectTransform == null)
        {
            return;
        }

        if (m_CanDrag)
        {
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

        //Right click up
        if (Input.GetMouseButtonUp(1))
        {
            if (m_IsTileRightClicked)
            {
                m_IsTileRightClicked = false;
                m_CanDrag = true;
            }
        }
    }
}
