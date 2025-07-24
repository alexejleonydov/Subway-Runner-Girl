using UnityEditor;
using UnityEngine;
using System.IO;

public class ProgressResetMenu
{
    [MenuItem("Tools/🔁 Reset game progress")]
    private static void ResetGameProgress()
    {
        // PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Видалення файлу збереження
        string savePath = System.IO.Path.Combine(Application.persistentDataPath, "save.json");
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Saves deleted: " + savePath);
        }

        Debug.Log("✅ Game progress Reset Successful!");
    }
}
