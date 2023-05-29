using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SC
{
    public abstract class State : ScriptableObject
    {
        [Header("State Info")]
        [SerializeField] protected string m_StateName = "";

        protected Statechart m_Statechart = null;

        public virtual void OnEnter(Statechart controller)
        {
            m_Statechart = controller;
        }

        public virtual void TriggerTransition<T>() where T : StateTransition, new()
        {
            T newTransition = new T();
            newTransition.Execute(m_Statechart);
        }

        public virtual void OnUpdate() { }

        public virtual void OnExit() { }
    }
}
