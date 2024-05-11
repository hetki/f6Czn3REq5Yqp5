using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class PersistenceEditorTool : EditorWindow
{

    [MenuItem("Hetki/Persistence/Clear Player Prefs")]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Hetki/Persistence/Delete Match State File")]
    public static void DeleteMatchStateFile()
    {
        string path = Application.persistentDataPath + "/matchState.json";
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Match state file deleted");
        }
        else
        {
            Debug.LogWarning("Could not find match state file");
        }
    }

    [MenuItem("Hetki/Persistence/Mass Clear")]
    public static void MassClear()
    {
        ClearPlayerPrefs();
        DeleteMatchStateFile();
    }

}
#endif
