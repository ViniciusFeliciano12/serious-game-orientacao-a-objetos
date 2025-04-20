using System.Collections.Generic;
using UnityEngine;
using static GameDatabase;

[System.Serializable]
public class ItemDatabase
{
    public string nome;
    public Texture icon;
    public SkillEnumerator skillID;

    public List<StringPair> propriedades = new();
    public List<StringPair> metodos = new();
}


[System.Serializable]
public class StringPair
{
    public string chave;
    public string valor;
}