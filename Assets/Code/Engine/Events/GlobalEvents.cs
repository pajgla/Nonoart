using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Global Events", menuName = "Game/Global Events")]
public class GlobalEvents : ScriptableObject
{
    // Gameplay
    public event UnityAction<GridTile, KeyCode> e_OnTileClicked;
    public event UnityAction<GridTile> e_OnTilePainted;

    //Logic
    private void Invoke_Internal<T>(UnityAction<T> eventToInvoke, T parameter)
    {
        if (eventToInvoke != null)
            eventToInvoke(parameter);
    }

    private void Invoke_Internal<T1,T2>(UnityAction<T1,T2> eventToInvoke, T1 parameter1, T2 parameter2)
    {
        if (eventToInvoke != null)
            eventToInvoke(parameter1, parameter2);
    }

    private void Invoke_Internal(UnityAction eventToInvoke)
    {
        if (eventToInvoke != null)
            eventToInvoke();
    }

    //Invocations

    public void Invoke_OnTilePainted(GridTile tile)
    {
        Invoke_Internal(e_OnTilePainted, tile);
    }

    public void Invoke_OnTileClicked(GridTile tile, KeyCode buttonIndex)
    {
        Invoke_Internal(e_OnTileClicked, tile, buttonIndex);
    }
}
