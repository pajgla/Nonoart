using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTileClickedEventArgs : EventArgs
{
    public int m_WidthIndex = 0;
    public int m_HeightIndex = 0;
}

public class GameModeControllerBase : MonoBehaviour
{
    event EventHandler<OnTileClickedEventArgs> m_OnTileClickedEventHandler;


}
