using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIViewModel
{
    public class ColorPickerViewModel : ViewModel
    {
        [SerializeField] FlexibleColorPicker m_ColorPickerRef = null;

        public FlexibleColorPicker GetColorPickerRef() { return m_ColorPickerRef; }
        public Color GetSelectedColor() { return GetColorPickerRef().GetColor(); }
    }
}
