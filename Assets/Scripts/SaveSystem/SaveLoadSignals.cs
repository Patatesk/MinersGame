using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class SaveLoadSignals 
{
    public static event Action Save;
    public static void Signal_Save()
    { Save?.Invoke(); }

    public static event Action Load;
    public static void Signal_Load()
    { Load?.Invoke(); }
    public static event Action Delete;

    public static void Signal_Delete()
    {
        Delete?.Invoke(); 
    }
}
