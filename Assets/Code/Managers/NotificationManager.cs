using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIViewModel;
using DG.Tweening;
using System;
using static NotificationManager;
using UnityEditor.TerrainTools;

public class NotificationManager : BaseSingleton<NotificationManager>
{
    public enum ENotificationType
    {
        Info,
        Warning,
        Error
    }

    [Serializable]
    public struct NotificationColorInfo
    {
        public NotificationManager.ENotificationType m_NotificationType;
        public Color m_NotificationColor;
        public Color m_TextColor;
    }

    private struct NotificationRequest
    {
        public ENotificationType m_Type;
        public string m_Text;

        public NotificationRequest(ENotificationType type, string text)
        {
            m_Type = type;
            m_Text = text;
        }
    }

    [Header("References")]
    [SerializeField] CentralNotificationViewModel m_NotificationViewModelRef = null;

    [Header("Animation")]
    [SerializeField] Vector3 m_StartingPosition = Vector3.zero;
    [SerializeField] Vector3 m_DestinationPosition = Vector3.zero;
    [SerializeField] float m_AnimationDuration = 1.0f;
    [SerializeField] float m_StayDuration = 2.0f;
    [SerializeField] Ease m_ShowAnimationEase = Ease.Linear;
    [SerializeField] Ease m_HideAnimationEase = Ease.Linear;

    [Header("Colors")]
    [SerializeField] List<NotificationColorInfo> m_NotificationColorInfos = new List<NotificationColorInfo>();

    //Viewmodel
    CentralNotificationViewModel m_NotificationViewModel = null;

    LinkedList<NotificationRequest> m_NotificationRequests = new LinkedList<NotificationRequest>();
    bool m_IsNotificationInProgress = false;

    protected override bool Awake()
    {
        if (base.Awake() == false)
        {
            return false;
        }

        m_NotificationViewModel = ViewModelHelper.SpawnAndInitialize(m_NotificationViewModelRef);
        m_NotificationViewModel.GetRectTransform().anchoredPosition = m_StartingPosition;

        return true;
    }

    private void Update()
    {
        if (m_NotificationRequests.Count == 0)
            return;

        if (m_IsNotificationInProgress) return;

        NotificationRequest nextRequest = m_NotificationRequests.First.Value;
        m_NotificationRequests.RemoveFirst();
        ProcessNotificationRequest(nextRequest);
    }

    public void RequestNotification(ENotificationType notificationType, string text)
    {
        NotificationRequest newRequest = new NotificationRequest(notificationType, text);
        m_NotificationRequests.AddLast(newRequest);
    }

    public void RequestImmediateNotification(string text, ENotificationType notificationType)
    {
        NotificationRequest newRequest = new NotificationRequest(notificationType, text);
        m_NotificationRequests.AddFirst(newRequest);
    }

    private void ProcessNotificationRequest(NotificationRequest request)
    {
        NotificationColorInfo colorInfo = new NotificationColorInfo();
        bool isFound = false;
        foreach (NotificationColorInfo info in m_NotificationColorInfos)
        {
            if (info.m_NotificationType == request.m_Type)
            {
                colorInfo = info;
                isFound = true;
                break;
            }
        }

        if (!isFound)
        {
            Debug.LogError("Notification Color info not found. Please add color info for " + request.m_Type.ToString() + " notification type");
            return;
        }

        m_NotificationViewModel.SetNotificationColorData(colorInfo);
        m_NotificationViewModel.SetNotificationText(request.m_Text);

        m_NotificationViewModel.GetRectTransform().DOAnchorPosY(m_DestinationPosition.y, m_AnimationDuration)
            .SetEase(m_ShowAnimationEase)
            .OnComplete(OnShowAnimationComplete);

        m_IsNotificationInProgress = true;
    }

    private void OnShowAnimationComplete()
    {
        StartCoroutine(NotificationStayAnimation());
    }

    private IEnumerator NotificationStayAnimation()
    {
        yield return new WaitForSeconds(m_StayDuration);
        StartHideNotification();
    }

    private void StartHideNotification()
    {
        m_NotificationViewModel.GetRectTransform().DOAnchorPosY(m_StartingPosition.y, m_AnimationDuration)
            .SetEase(m_HideAnimationEase)
            .OnComplete(OnNotificationDone);
    }

    public void OnNotificationDone()
    {
        m_IsNotificationInProgress = false;
    }
}
