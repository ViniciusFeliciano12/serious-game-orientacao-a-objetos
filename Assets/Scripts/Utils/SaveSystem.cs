using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string saveFile = Path.Combine(Application.persistentDataPath, "save.json");

    // Salvar GameDatabase em JSON
    public static void Save(GameDatabase data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFile, json);
    }

    // Carregar GameDatabase de JSON
    public static bool Load(GameDatabase data)
    {
        if (!File.Exists(saveFile))
            return false;

        string json = File.ReadAllText(saveFile);
        JsonUtility.FromJsonOverwrite(json, data);
        return true;
    }

    // Verifica se existe save
    public static bool SaveExists()
    {
        return File.Exists(saveFile);
    }
}
