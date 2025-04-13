using UnityEngine;
using static GameDatabase;

[System.Serializable]
public class ItemDatabase{
    public int index;
    public Texture icon;
    public SkillEnumerator skillID; 
    public string objectSerialized;
}