using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelCountWidget : MonoBehaviour
{
    [SerializeField] GameObject m_PixelCountTextPrefab = null;

    bool m_IsVertical = false;

    public void SetIsVertical(bool isVertical)
    {
        m_IsVertical = isVertical;
    }

    public void AdjustPositionRelativeTo(RectTransform other)
    {
        RectTransform rectTransformComponent = GetComponent<RectTransform>();
        rectTransformComponent.SetParent(other, false);
        if (m_IsVertical)
        {
            rectTransformComponent.Rotate(new Vector3(0.0f, 0.0f, -90.0f));
            rectTransformComponent.anchoredPosition = new Vector2(0.0f, (rectTransformComponent.sizeDelta.x / 2 + other.sizeDelta.x / 2));
        }
        else
        {
            rectTransformComponent.anchoredPosition = new Vector2(- (rectTransformComponent.sizeDelta.x / 2 + other.sizeDelta.x / 2), 0.0f);
        }
    }

    public void AddPixelCount(int count)
    {
        GameObject newObject = Instantiate(m_PixelCountTextPrefab);
        newObject.transform.SetParent(transform, false);

        if (m_IsVertical)
        {
            newObject.GetComponent<RectTransform>().Rotate(new Vector3(0.0f, 0.0f, 90.0f));
        }

        TMPro.TextMeshProUGUI textComponent = newObject.GetComponent<TMPro.TextMeshProUGUI>();
        if (textComponent == null)
        {
            Debug.LogError("TextMeshProUGUI component is missing from pixel count prefab");
            Destroy(newObject);
            return;
        }

        textComponent.text = count.ToString();
    }
}
