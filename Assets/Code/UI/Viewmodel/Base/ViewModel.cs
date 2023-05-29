using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UIViewModel
{
    [RequireComponent(typeof(Canvas))]
    public abstract class ViewModel : MonoBehaviour
    {
        public virtual void Initialize()
        {
            // Override if not applicable
            Canvas canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
        }

        public Canvas GetViewModelCanvas()
        {
            return GetComponent<Canvas>();
        }

        public void ChangeViewModelVisibility(bool visible)
        {
            GetViewModelCanvas().enabled = visible;
        }
    }

    public static class ViewModelHelper
    {
        public static T SpawnAndInitialize<T>(T vmCopy) where T : ViewModel
        {
            T newVM = Object.Instantiate(vmCopy);
            newVM.Initialize();
            return newVM;
        }
    }
}
