using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CategorySelectionWidget : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI m_CategoryNameText = null;
    [SerializeField] NonogramSet m_NonogramSet = null;

    public event Action<NonogramSet> OnCategorySelectedEvent;

    public Button GetAssociatedButton()
    {
        return GetComponent<Button>();
    }

    private void Start()
    {
        GetAssociatedButton().onClick.AddListener(OnCategorySelected);
    }

    public void Init(NonogramSet nonogramSet)
    {
        if (nonogramSet == null)
        {
            Debug.LogError("Empty Nonogram Set provided. This widget will be destroyed.");
            Destroy(gameObject);
        }

        m_CategoryNameText.text = nonogramSet.GetName();
        m_NonogramSet = nonogramSet;
    }

    public void OnCategorySelected()
    {
        OnCategorySelectedEvent(m_NonogramSet);
    }
}
