using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelCountWidget : MonoBehaviour
{
    struct ClueInstance
    {
        public int m_PixelCount;
        public TMPro.TextMeshProUGUI m_TextComponent;

        public ClueInstance(int count, TMPro.TextMeshProUGUI component)
        {
            m_PixelCount = count;
            m_TextComponent = component;
        }
    }

    [SerializeField] GameObject m_ClueTextPrefab = null;

    [Header("Colors")]
    [SerializeField] Color m_SolvedClueColor = Color.white;
    [SerializeField] Color m_UnsolvedClueColor = Color.white;
    
    List<ClueInstance> m_SpawnedClueInstances = new List<ClueInstance>();

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
            //#TODO: Remove magic numbers
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
        GameObject newObject = Instantiate(m_ClueTextPrefab);
        newObject.transform.SetParent(transform, false);

        if (m_IsVertical)
        {
            //#TODO: remove magic numbers
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

        ClueInstance newClue = new ClueInstance(count, textComponent);
        ChangeClueColor(newClue, false);

        m_SpawnedClueInstances.Add(newClue);
    }

    public void SetClueSolved(int clueIndex)
    {
        if (clueIndex > m_SpawnedClueInstances.Count - 1)
        {
            Debug.LogError("Invalid clue index provided");
            return;
        }

        ClueInstance clue = m_SpawnedClueInstances[clueIndex];
        ChangeClueColor(clue, true);
    }

    private void ChangeClueColor(ClueInstance clue, bool isSolved)
    {
        TMPro.TextMeshProUGUI textComponent = clue.m_TextComponent;
        if (isSolved)
        {
            textComponent.color = m_SolvedClueColor;
        }
        else
        {
            textComponent.color = m_UnsolvedClueColor;
        }
    }
}
