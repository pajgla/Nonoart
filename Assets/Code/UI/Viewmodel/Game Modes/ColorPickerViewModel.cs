using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIViewModel
{
    public class ColorPickerViewModel : ViewModel
    {
        [SerializeField] ColorPicker m_ColorPickerRef = null;

        public override void Initialize()
        {
            base.Initialize();

            m_ColorPickerRef = Instantiate(m_ColorPickerRef, gameObject.transform);
        }

        public ColorPicker GetColorPickerRef() { return m_ColorPickerRef; }

        public Color GetSelectedColor()
        {
            return ColorPicker.GetSelectedColor();
        }

        public void ShowColorPickerWindow()
        {
            ColorPicker.Create(false);
        }
    }
}
