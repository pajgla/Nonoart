using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class LevelSelectionWidget : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnLevelButtonClick);
    }

    public Image GetImageComponent()
    {
        return GetComponent<Image>();
    }

    public void SetLevelImage(Texture2D texture)
    {
        Rect rect = new Rect(0.0f, 0.0f, texture.width, texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        Sprite newSprite = Sprite.Create(texture, rect, pivot, 100.0f);
        GetImageComponent().sprite = newSprite;
    }

    private void OnLevelButtonClick()
    {

    }
}
