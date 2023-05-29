using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Global Events", menuName = "Game/Global Events")]
public class GlobalEvents : ScriptableObject
{
    // Gameplay
    public event UnityAction<GridTile> e_OnTileClicked;


    public void OnTileClicked(GridTile tile)
    {
        e_OnTileClicked(tile);
    }
}
