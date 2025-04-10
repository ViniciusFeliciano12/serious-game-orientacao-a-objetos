using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "Scriptables/Database", order = 1)]
public class GameDatabase : ScriptableObject
{
    public List<ItemDatabase> itemObjects;

    public void ResetDatabase(){
        itemObjects = new();
    }

    public bool VerifyItemFound(ItemEnumerator itemID){
        return itemObjects.Any(item => itemID.Equals(item.itemID));
    }

    public bool VerifyItemLearned(ItemEnumerator itemID){
        return itemObjects.Any(item => itemID.Equals(item.itemID) && item.recipeLearned);
    }

    public void LearnItem(ItemEnumerator itemID){
        itemObjects.FirstOrDefault(item => item.itemID.Equals(itemID)).recipeLearned = true;
    }

    public void AddItem(ItemDatabase item){
        itemObjects.Add(item);
    }

    public enum ItemEnumerator{
        Key,
        Sword
    }
}