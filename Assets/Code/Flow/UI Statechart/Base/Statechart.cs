using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SC
{
    [Serializable]
    public struct StateProperty
    {
        public bool m_InitialState;
        public State m_StateRef;
    }

    public abstract class Statechart : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] State m_InitialState = null;

        [Header("Debug")]
        [SerializeField] protected State m_ActiveState = null;

        protected virtual void Awake()
        {
            m_ActiveState = null;
        }

        private void Start()
        {
            if (m_InitialState != null)
            {
                ChangeActiveState(m_InitialState);
            }
            else
            {
                Debug.LogError("Initial State not referenced");
            }
        }

        public virtual void TransitionTo<T>() where T : State, new()
        {
            T newState = new T();
            ChangeActiveState(newState);
        }

        protected virtual void ChangeActiveState(State state)
        {
            if (m_ActiveState == state)
            {
                Debug.LogError("Trying to transition to an active state");
                return;
            }

            if (m_ActiveState != null)
            {
                m_ActiveState.OnExit();
            }

            m_ActiveState = state;
            m_ActiveState.OnEnter(this);
        }

        private void Update()
        {
            if (m_ActiveState != null)
            {
                m_ActiveState.OnUpdate();
            }
        }
    }
}
