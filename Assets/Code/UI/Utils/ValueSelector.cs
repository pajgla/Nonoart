using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIValueSelector : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    TextMeshProUGUI m_ValueDisplay = null;
    [SerializeField]
    Button m_IncreaseButton = null;
    [SerializeField]
    Button m_DecreaseButton = null;

    [Header("Finetuning")]
    [SerializeField] float m_Step = 1.0f;
    [SerializeField] bool m_RoundValue = false;
    [SerializeField] float m_DefaultValue = 1.0f;
    [SerializeField] float m_MaxValue = 10.0f;
    [SerializeField] float m_MinValue = 0.0f;

    float m_Value = 0.0f;

    void Start()
    {
        m_IncreaseButton.onClick.AddListener(IncreaseValue);
        m_DecreaseButton.onClick.AddListener(DecreaseValue);

        m_Value = m_DefaultValue;
        UpdateValueDisplay();
    }

    public void IncreaseValue()
    {
        m_Value += m_Step;

        if (m_Value >= m_MaxValue)
        {
            m_Value = m_MaxValue;
            m_IncreaseButton.interactable = false;
        }

        m_DecreaseButton.interactable = true;

        UpdateValueDisplay();
    }

    public void DecreaseValue()
    {
        m_Value -= m_Step;

        if (m_Value < m_MinValue)
        {
            m_Value = m_MinValue;
            m_DecreaseButton.interactable = false;
        }

        m_IncreaseButton.interactable = true;

        UpdateValueDisplay();
    }

    public void UpdateValueDisplay()
    {
        if (m_RoundValue)
        {
            m_ValueDisplay.text = ((int)m_Value).ToString();
        }
        else
        {
            m_ValueDisplay.text = m_Value.ToString();
        }
    }

    public float GetValue() { return m_Value; }
}
