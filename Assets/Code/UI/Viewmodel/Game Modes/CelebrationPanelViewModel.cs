using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace UIViewModel
{
    public class CelebrationPanelViewModel : ViewModel
    {
        [Header("References")]
        [SerializeField] TextMeshProUGUI m_NonogramNameText = null;
        [SerializeField] TextMeshProUGUI m_TotalTimeText = null;
        [SerializeField] Image m_NonogramTexture = null;
        [SerializeField] Button m_ContinueButton = null;

        [SerializeField] string m_TotalTimePrefix = "Time:";

        public void SetNonogramName(string nonogramName)
        {
            m_NonogramNameText.text = nonogramName;
        }

        public void SetTotalTimeTaken(float totalTimeInSeconds)
        {
            float minutes = totalTimeInSeconds / 60;
            float seconds = totalTimeInSeconds % 60;

            string formattedTime = String.Format("{0:00}:{1:00}", minutes, seconds);

            m_TotalTimeText.text = m_TotalTimePrefix + formattedTime;
        }

        public void SetNonogramTexture(Texture2D texture)
        {
            Rect rect = new Rect(0.0f, 0.0f, texture.width, texture.height);
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            Sprite newSprite = Sprite.Create(texture, rect, pivot, 100.0f);
            m_NonogramTexture.sprite = newSprite;
            m_NonogramTexture.preserveAspect = true;
        }

        public Button GetContinueButton()
        {
            return m_ContinueButton;
        }
    }
}
