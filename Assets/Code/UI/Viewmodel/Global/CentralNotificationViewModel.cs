using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIViewModel
{
    public class CentralNotificationViewModel : ViewModel
    {
        [Header("References")]
        [SerializeField] RectTransform m_NotificationRectTransform = null;
        [SerializeField] TextMeshProUGUI m_NotificationText = null;
        [SerializeField] Image m_NotificationBackground = null;



        public override void Initialize()
        {
            base.Initialize();
        }

        public RectTransform GetRectTransform() { return m_NotificationRectTransform; }
        public void SetNotificationText(string text)
        {
            m_NotificationText.text = text;
        }

        public void SetNotificationColorData(NotificationManager.NotificationColorInfo colorInfo)
        {
            m_NotificationBackground.color = colorInfo.m_NotificationColor;
            m_NotificationText.color = colorInfo.m_TextColor;
        }
    }
}
