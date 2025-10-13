using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "Scriptables/Database", order = 1)]
public class GameDatabase : ScriptableObject
{
    [SerializeField] private List<SkillDatabase> skillTreeObjects;
    [SerializeField] private List<ItemDatabase> itemInventoryObjects;
    [SerializeField] private List<string> dialoguesAlreadyPlayed;
    [SerializeField] private List<int> torchesAlreadyLit;
    [SerializeField] private Player player;

    public void ResetDatabase()
    {
        itemInventoryObjects = new();
        skillTreeObjects = new();
        dialoguesAlreadyPlayed = new();
        torchesAlreadyLit = new();
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

    public void EditItemDatabase(ItemDatabase item, int index)
    {
        itemInventoryObjects[index].nome = item.nome;
        itemInventoryObjects[index].metodos = item.metodos;
        itemInventoryObjects[index].propriedades = item.propriedades;
        itemInventoryObjects[index].itemActive = item.itemActive;
        itemInventoryObjects[index].skillID = item.skillID;
    }

    public void RemoveItemDatabase(ItemDatabase item)
    {
        itemInventoryObjects.Remove(item);
    }

    #endregion

    #region Torches

    public void SaveTorchLit(int torchId)
    {
        torchesAlreadyLit.Add(torchId);
    }

    public bool TorchAlreadyLit(int torchId)
    {
        return torchesAlreadyLit.Exists(item => item.Equals(torchId));
    }

    public int QuantityTorchesLit()
    {
        return torchesAlreadyLit.Count();
    }

    #endregion

    #region Dialogues

    public void SaveDialoguePlayed(string dialogueGraph)
    {
        dialoguesAlreadyPlayed.Add(dialogueGraph);
    }

    public bool DialogueAlreadyPlayed(string dialogueGraph)
    {
        return dialoguesAlreadyPlayed.Exists(item => item.Equals(dialogueGraph));
    }

    #endregion

    #region Skill

    public int ReturnSkillCount()
    {
        return skillTreeObjects.Count;
    }

    public int ReturnSkillLearnedCount()
    {
        return skillTreeObjects.Where(item => item.recipeLearned).Count();
    }

    public bool VerifySkillFound(SkillEnumerator itemID)
    {
        return skillTreeObjects.Any(item => itemID.Equals(item.itemID));
    }

    public bool VerifySkillLearned(SkillEnumerator itemID){
        return skillTreeObjects.Any(item => itemID.Equals(item.itemID) && item.recipeLearned);
    }

    public bool VerifyItemAlreadyCreated(SkillEnumerator itemID)
    {
        return skillTreeObjects.Any(item => itemID.Equals(item.itemID) && item.createdFirstTime);
    }

    public void CreateSkillFirstTime(SkillEnumerator itemID)
    {
        skillTreeObjects.FirstOrDefault(item => item.itemID.Equals(itemID)).createdFirstTime = true;
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
        Shield,
        IsReusable,
        Set,
        Rag,
        Oil,
        Ignitor
    }
}