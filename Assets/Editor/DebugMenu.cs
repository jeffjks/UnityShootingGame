using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DebugMenu : EditorWindow
{
    [MenuItem("Tools/Invincible Mod")]
    private static void ToggleInvincibleMod()
    {
        var value = EditorPrefsGetBool("InvincibleMod", false);
        value = !value;
        EditorPrefsSetBool("InvincibleMod", value);
        Menu.SetChecked("Tools/Invincible Mod", value);
    }
    
    [MenuItem("Tools/Invincible Mod", true)]
    private static bool ToggleInvincibleModValidate()
    {
        var value = EditorPrefsGetBool("InvincibleMod", false);
        Menu.SetChecked("Tools/Invincible Mod", value);
        return true;
    }
    
    [MenuItem("Tools/Network Available")]
    private static void ToggleNetworkAvailable()
    {
        var value = EditorPrefsGetBool("NetworkAvailable", false);
        value = !value;
        EditorPrefsSetBool("NetworkAvailable", value);
        Menu.SetChecked("Tools/Network Available", value);
    }
    
    [MenuItem("Tools/Network Available", true)]
    private static bool ToggleNetworkAvailableValidate()
    {
        var value = EditorPrefsGetBool("NetworkAvailable", false);
        Menu.SetChecked("Tools/Network Available", value);
        return true;
    }
    
    [MenuItem("Tools/Generate Json File")]
    private static void ToggleGenerateJsonFile()
    {
        var value = EditorPrefsGetBool("GenerateJsonFile", false);
        value = !value;
        EditorPrefsSetBool("GenerateJsonFile", value);
        Menu.SetChecked("Tools/Generate Json File", value);
    }
    
    [MenuItem("Tools/Generate Json File", true)]
    private static bool ToggleGenerateJsonFileValidate()
    {
        var value = EditorPrefsGetBool("GenerateJsonFile", false);
        Menu.SetChecked("Tools/Generate Json File", value);
        return true;
    }

    private static bool EditorPrefsGetBool(string key, bool defaultValue)
    {
        return EditorPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
    }

    private static void EditorPrefsSetBool(string key, bool defaultValue)
    {
        EditorPrefs.SetInt(key, defaultValue ? 1 : 0);
    }
}