using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "Scriptables/Database", order = 1)]
public class GameDatabase : ScriptableObject
{
    public List<SkillDatabase> skillTreeObjects;
    public List<ItemDatabase> itemInventoryObjects;
    public Player player;

    public void ResetDatabase(){
        skillTreeObjects = new();
        itemInventoryObjects = new();
        player = new();
    }

    #region Player

    public void SavePlayerPosition(Vector3 position){
        player.position = position;
    }

    public Vector3 GetPlayerPosition(){
        return player.position;
    }

    #endregion

    #region Inventory

    public List<ItemDatabase> GetInventory(){
        return itemInventoryObjects;
    }

    public void AddItemDatabase(ItemDatabase item){
        itemInventoryObjects.Add(item);
    }

    #endregion

    #region Skill

    public bool VerifySkillFound(SkillEnumerator itemID){
        return skillTreeObjects.Any(item => itemID.Equals(item.itemID));
    }

    public bool VerifySkillLearned(SkillEnumerator itemID){
        return skillTreeObjects.Any(item => itemID.Equals(item.itemID) && item.recipeLearned);
    }

    public void LearnSkill(SkillEnumerator itemID){
        skillTreeObjects.FirstOrDefault(item => item.itemID.Equals(itemID)).recipeLearned = true;
    }

    public void AddSkill(SkillDatabase item){
        skillTreeObjects.Add(item);
    }

    #endregion

    public enum SkillEnumerator{
        Key,
        Weapon,
        Sword
    }
}