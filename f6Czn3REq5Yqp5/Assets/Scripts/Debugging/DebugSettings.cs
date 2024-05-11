using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Project DebugSettings - Enables or disables logging
/// </summary>
public static class DebugSettings
{
    private static bool logOverride = false;

    //Only log in editor or when overridden
    public static bool LogState() 
    {
#if UNITY_EDITOR
            return true;
#endif

#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_WIN
#pragma warning disable CS0162 // Unreachable code detected - code is reachable in builds
        if (logOverride)
                    return true;
#pragma warning restore CS0162 // Unreachable code detected - code is reachable in builds
#endif
        return false;
    }
}
