using System.IO;
using UnityEngine;

public static class SaveSystem
{
    // RETORNADO PELA LÓGICA DO JOGO EM EXECUÇÃO
    private static string GetSavePath()
    {
#if UNITY_EDITOR
        return GetEditorSavePath();
#else
            return GetBuildSavePath();
#endif
    }

    // CAMINHO ESPECÍFICO DO SAVE NO EDITOR
    public static string GetEditorSavePath()
    {
        // Salva em uma pasta dentro do projeto para fácil acesso e isolamento.
        return Path.Combine(Application.dataPath, "EditorSaves", "save.json");
    }

    // CAMINHO ESPECÍFICO DO SAVE NA BUILD
    public static string GetBuildSavePath()
    {
        // Caminho persistente padrão do usuário.
        return Path.Combine(Application.persistentDataPath, "save.json");
    }

    // --- Métodos de Jogo (Save, Load, Exists) ---
    // Esses não mudam e continuarão usando a lógica do GetSavePath()

    public static void Save(GameDatabase data)
    {
        string saveFile = GetSavePath();
        Directory.CreateDirectory(Path.GetDirectoryName(saveFile));
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFile, json);
    }

    public static bool Load(GameDatabase data)
    {
        string saveFile = GetSavePath();
        if (!File.Exists(saveFile)) return false;
        try
        {
            string json = File.ReadAllText(saveFile);
            JsonUtility.FromJsonOverwrite(json, data);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao carregar o arquivo de save: {e.Message}");
            return false;
        }
    }

    public static bool SaveExists()
    {
        return File.Exists(GetSavePath());
    }

    // --- Métodos de Exclusão para Ferramentas ---
    // Estes são os métodos que nossa ferramenta do editor vai usar.

    private static void DeleteFileAtPath(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Arquivo de save deletado com sucesso de: {path}");
        }
        else
        {
            Debug.LogWarning($"Tentativa de deletar save, mas nenhum arquivo foi encontrado em: {path}");
        }
    }

    public static void DeleteEditorSave()
    {
        DeleteFileAtPath(GetEditorSavePath());
    }

    public static void DeleteBuildSave()
    {
        DeleteFileAtPath(GetBuildSavePath());
    }
}