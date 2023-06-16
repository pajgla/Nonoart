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

        //#TODO: Unity doesnt allow exposing sorting layers to members and we also can't know sorting layer's ID so we have
        //to use strings instead. Find a better solution for this
        public void SetSortingLayer(string name)
        {
            GetViewModelCanvas().sortingLayerName = name;
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
