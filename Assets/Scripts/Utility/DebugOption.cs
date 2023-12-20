using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DebugOption
{
#if UNITY_EDITOR
    public static bool InvincibleMod => EditorPrefsGetBool("InvincibleMod", false);
    public static bool NetworkAvailable => EditorPrefsGetBool("NetworkAvailable", false);
    public static bool GenerateJsonFile => EditorPrefsGetBool("GenerateJsonFile", false);
    public static bool LoadTempReplayFile => EditorPrefsGetBool("LoadTempReplayFile", false);
    public static int SceneMode => EditorPrefsGetInt("SceneMode", 0);
    private static bool EditorPrefsGetBool(string key, bool defaultValue)
    {
        return EditorPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
    }
    private static int EditorPrefsGetInt(string key, int defaultValue)
    {
        return EditorPrefs.GetInt(key, defaultValue);
    }
#else
    public static bool InvincibleMod => false;
    public static bool NetworkAvailable => false;
    public static bool GenerateJsonFile => false;
    public static bool LoadTempReplayFile => false;
    public static int SceneMode => 0;
#endif
}