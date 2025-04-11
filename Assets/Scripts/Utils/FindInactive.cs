using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class FindInactive
{
    /// <summary>
    /// Procura um GameObject na cena com o nome especificado, mesmo que esteja desativado.
    /// </summary>
    /// <param name="name">Nome do GameObject a ser procurado</param>
    /// <returns>O GameObject encontrado ou null, se não encontrar</returns>
    public static GameObject Find(string name)
    {
        return GameObject.FindObjectsOfType<Transform>(true).FirstOrDefault(transform => transform.name == name).gameObject;
    }

    /// <summary>
    /// Procura um GameObject de UI na cena com o nome especificado, mesmo que esteja desativado.
    /// </summary>
    /// <param name="name">Nome do GameObject a ser procurado</param>
    /// <returns>O GameObject encontrado ou null, se não encontrar</returns>

    public static GameObject FindUIElement(string name)
    {
        return GameObject.FindObjectsOfType<RectTransform>(true).FirstOrDefault(transform => transform.name == name).gameObject;
    }

    /// <summary>
    /// Procura todos os GameObjects com o nome especificado, mesmo que estejam desativados.
    /// </summary>
    /// <param name="name">Nome do GameObject a ser procurado</param>
    /// <returns>Uma lista com todos os GameObjects encontrados</returns>
    public static List<GameObject> FindAll(string name)
    {
        Transform[] allTransforms = GameObject.FindObjectsOfType<Transform>(true);
        List<GameObject> result = new List<GameObject>();

        foreach (Transform t in allTransforms)
        {
            if (t.name == name)
                result.Add(t.gameObject);
        }

        return result;
    }
}
