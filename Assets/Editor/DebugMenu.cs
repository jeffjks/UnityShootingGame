using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DebugMenu : EditorWindow
{
    private static readonly string[] sceneModeRadioMenus =
    {
        "Tools/Scene Mode/Default",
        "Tools/Scene Mode/Debug Player",
        "Tools/Scene Mode/Debug Enemy"
    };
    
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

    [MenuItem("Tools/Scene Mode/Default")]
    private static void RadioButtonSceneModeDefault()
    {
        EditorPrefsSetInt("SceneMode", 0);
        foreach (var menuPath in sceneModeRadioMenus)
        {
            Menu.SetChecked(menuPath, false);
        }
        Menu.SetChecked("Tools/Scene Mode/Default", true);
    }
    
    [MenuItem("Tools/Scene Mode/Default", true)]
    private static bool RadioButtonSceneModeDefaultValidate()
    {
        var value = EditorPrefsGetInt("SceneMode", 0);
        Menu.SetChecked("Tools/Scene Mode/Default", value == 0);
        return true;
    }

    [MenuItem("Tools/Scene Mode/Debug Player")]
    private static void RadioButtonSceneModePlayerDebug()
    {
        EditorPrefsSetInt("SceneMode", 1);
        foreach (var menuPath in sceneModeRadioMenus)
        {
            Menu.SetChecked(menuPath, false);
        }
        Menu.SetChecked("Tools/Scene Mode/Debug Player", true);
    }
    
    [MenuItem("Tools/Scene Mode/Debug Player", true)]
    private static bool RadioButtonSceneModePlayerDebugValidate()
    {
        var value = EditorPrefsGetInt("SceneMode", 0);
        Menu.SetChecked("Tools/Scene Mode/Debug Player", value == 1);
        return true;
    }

    [MenuItem("Tools/Scene Mode/Debug Enemy")]
    private static void RadioButtonSceneModeEnemyDebug()
    {
        EditorPrefsSetInt("SceneMode", 2);
        foreach (var menuPath in sceneModeRadioMenus)
        {
            Menu.SetChecked(menuPath, false);
        }
        Menu.SetChecked("Tools/Scene Mode/Debug Enemy", true);
    }
    
    [MenuItem("Tools/Scene Mode/Debug Enemy", true)]
    private static bool RadioButtonSceneModeEnemyDebugValidate()
    {
        var value = EditorPrefsGetInt("SceneMode", 0);
        Menu.SetChecked("Tools/Scene Mode/Debug Enemy", value == 2);
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

    private static int EditorPrefsGetInt(string key, int defaultValue)
    {
        return EditorPrefs.GetInt(key, defaultValue);
    }

    private static void EditorPrefsSetInt(string key, int defaultValue)
    {
        EditorPrefs.SetInt(key, defaultValue);
    }
}