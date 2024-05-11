using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class PersistenceEditorTool : EditorWindow
{

    /// <summary>
    /// Clears all player prefs
    /// </summary>
    [MenuItem("Hetki/Persistence/Clear Player Prefs")]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    /// <summary>
    /// Deletes match state file
    /// </summary>
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

    /// <summary>
    /// Clears all prefs and deletes match state file
    /// </summary>
    [MenuItem("Hetki/Persistence/Mass Clear")]
    public static void MassClear()
    {
        ClearPlayerPrefs();
        DeleteMatchStateFile();
    }

}
#endif
