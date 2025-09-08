using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "Scriptables/Database", order = 1)]
public class GameDatabase : ScriptableObject
{
    [SerializeField] private List<SkillDatabase> skillTreeObjects;
    [SerializeField] private List<ItemDatabase> itemInventoryObjects;
    [SerializeField] private Player player;

    public void ResetDatabase(){
        itemInventoryObjects = new();
        skillTreeObjects = new();
        player = new();
    }

    public void ResetDatabasePartial()
    {
        player = new();
    }

    #region Player

    public void SavePlayerPosition(Vector3 position)
    {
        player.position = position;
    }

    public Vector3 GetPlayerPosition(){
        return player.position;
    }

    public int UpdatePlayerLifes(int damageTaken)
    {
        player.life -= damageTaken;
        return ReturnPlayerLifes();
    }

    public int ReturnPlayerLifes()
    {
        return player.life;
    }

    #endregion

    #region Inventory

    public List<ItemDatabase> GetInventory(){
        return itemInventoryObjects;
    }

    public void AddItemDatabase(ItemDatabase item){
        itemInventoryObjects.Add(item);
    }

    public void RemoveItemDatabase(ItemDatabase item)
    {
        itemInventoryObjects.Remove(item);
    }

    #endregion

    #region Skill

    public int ReturnSkillCount()
    {
        return skillTreeObjects.Count;
    }

    public bool VerifySkillFound(SkillEnumerator itemID)
    {
        return skillTreeObjects.Any(item => itemID.Equals(item.itemID));
    }

    public bool VerifySkillLearned(SkillEnumerator itemID){
        return skillTreeObjects.Any(item => itemID.Equals(item.itemID) && item.recipeLearned);
    }

    public void LearnSkill(SkillEnumerator itemID){
        skillTreeObjects.FirstOrDefault(item => item.itemID.Equals(itemID)).recipeLearned = true;
    }

    public void AddSkill(SkillDatabase item)
    {
        skillTreeObjects.Add(item);
    }

    #endregion

    public enum SkillEnumerator
    {
        Key,
        Equipment,
        Sword,
        Crowbar,
        Torch,
        Gravel,
        Axe,
        Shield
    }
}