using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreationGameMode : MonoBehaviour
{
    int m_GridWidth = 0;
    int m_GridHeight = 0;

    [Header("References")]
    [SerializeField] GridSpawner m_GridSpawner = null;
    [SerializeField] GameObject m_ConfigurationPanel = null;
    [SerializeField] ValueSelector m_WidthValueSelector = null;
    [SerializeField] ValueSelector m_HeightValueSelector = null;

    [Header("Debug")]
    [SerializeField] Color m_PickedColor = Color.white;

    public void OnStartButtonPressed()
    {
        m_ConfigurationPanel.SetActive(false);

        m_GridWidth = (int)m_WidthValueSelector.GetValue();
        m_GridHeight = (int)m_HeightValueSelector.GetValue();

        StartCoroutine(m_GridSpawner.SpawnGrid(m_GridWidth, m_GridHeight));
    }

    public void OpenColorPicker()
    {

    }
}
