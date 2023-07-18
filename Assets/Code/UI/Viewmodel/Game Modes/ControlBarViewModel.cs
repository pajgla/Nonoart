using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIViewModel
{
    public class ControlBarViewModel : ViewModel
    {
        [Header("References")]
        [SerializeField] Image m_DrawingTypeImage = null;
        [SerializeField] TMPro.TextMeshProUGUI m_TimeText = null;
        [SerializeField] TMPro.TextMeshProUGUI m_LivesText = null;
        [SerializeField] Button m_PauseButton = null;
        [SerializeField] Button m_CreateNonogramButton = null; 

        [Header("Data")]
        [SerializeField] Sprite m_FreeDrawingTypeImage = null;
        [SerializeField] Sprite m_LineDrawingTypeSprite = null;
        [SerializeField] string m_TimePrefix = "Time: ";
        [SerializeField] string m_LivesPrefix = "Lives: ";

        public void SetDrawingModeImage(DrawingController.EDrawingType drawingType)
        {
            Sprite spriteToUse = null;
            if (drawingType == DrawingController.EDrawingType.Free)
            {
                spriteToUse = m_FreeDrawingTypeImage;
            }
            else
            {
                spriteToUse = m_LineDrawingTypeSprite;
            }

            m_DrawingTypeImage.sprite = spriteToUse;
        }

        public void SetTime(float time)
        {
            string formattedString = m_TimePrefix;
            float minutes = time / 60;
            float seconds = time % 60;

            string formattedTime = String.Format("{0:00}:{1:00}", minutes, seconds);

            formattedString += formattedTime;
            m_TimeText.text = formattedString;
        }

        public void SetLives(int lives)
        {
            m_LivesText.text = m_LivesPrefix + lives;
        }

        public Button GetPauseButton()
        {
            return m_PauseButton;
        }

        public void ChangeLivesTextVisibility(bool isVisible)
        {
            m_LivesText.gameObject.SetActive(isVisible);
        }

        public void ChangeTimeTextVisibility(bool isVisible)
        {
            m_TimeText.gameObject.SetActive(isVisible);
        }

        public void ChangeCreateButtonVisibility(bool isVisible)
        {
            m_CreateNonogramButton.gameObject.SetActive(isVisible);
        }

        public Button GetCreateNonogramButton()
        {
            return m_CreateNonogramButton;
        }
    }
}
