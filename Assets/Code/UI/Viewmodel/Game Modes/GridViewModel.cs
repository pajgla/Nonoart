using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIViewModel
{
    public class GridViewModel : ViewModel
    {
        [SerializeField] RectTransform m_GridHolder = null;

        public RectTransform GetGridHolder() { return m_GridHolder; }
    }
}
