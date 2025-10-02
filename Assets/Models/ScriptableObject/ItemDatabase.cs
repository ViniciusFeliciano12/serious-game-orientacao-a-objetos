using System.Collections.Generic;
using UnityEngine;
using static GameDatabase;

[System.Serializable]
public class ItemDatabase
{
    public string nome;
    public SkillEnumerator skillID;
    public bool itemActive;

    public string iconPath;

    public List<StringPair> propriedades = new();
    public List<StringPair> metodos = new();

    [System.NonSerialized]
    private Texture _icon;

    public Texture icon
    {
        get
        {
            if (_icon == null && !string.IsNullOrEmpty(iconPath))
            {
                _icon = Resources.Load<Texture>(iconPath);
            }
            return _icon;
        }
    }

    public void UpdateIconPath(string newPath)
    {
        iconPath = newPath;
        _icon = null;
    }
}


[System.Serializable]
public class StringPair
{
    public string chave;
    public string valor;
}

public class StringPairComparer : IEqualityComparer<StringPair>
{
    public bool Equals(StringPair x, StringPair y)
    {
        return x.chave == y.chave && x.valor == y.valor;
    }

    public int GetHashCode(StringPair obj)
    {
        return obj.chave.GetHashCode() ^ obj.valor.GetHashCode();
    }
}