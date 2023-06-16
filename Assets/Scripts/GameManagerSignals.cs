using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GameManagerSignals
{

    public static event Action<Action<GameObject, GameObject, GameObject>> SetButtons;
    public static void Signal_SetButtons(Action<GameObject,GameObject,GameObject> callBack)
    { SetButtons?.Invoke(callBack); }

    public static event Action CloseMountains;
    public static void Signal_CloseMountains()
    { CloseMountains?.Invoke(); }
    
}
