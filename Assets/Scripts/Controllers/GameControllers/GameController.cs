using System;
using System.Collections.Generic;
using UnityEngine;
using static GameDatabase;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public GameDatabase Database; 
    private bool PlayerCanMove = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }  
    }

    void Start()
    {
        //Database.ResetDatabase();
        UIController.Instance.UpdateHUD(lifes: Database.ReturnPlayerLifes(), scrolls: Database.ReturnSkillCount());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePlayerLifes(int damageTaken)
    {
        var lifesRemaining = Database.UpdatePlayerLifes(damageTaken);
        UIController.Instance.UpdateHUD(lifes: lifesRemaining);

        if (lifesRemaining <= 0)
        {
            //logica para resetar a fase
        }
    }

    public void UpdatePlayerMovement(){
        PlayerCanMove = !PlayerCanMove;
    }

    public bool CanPlayerMove(){
        return PlayerCanMove;
    }

    public void UpdatePlayerPosition(Vector3 position){
        Database.SavePlayerPosition(position);
    }

    public Vector3 GetPlayerPosition(){
        return Database.GetPlayerPosition();
    }

    public List<ItemDatabase> GetInventory(){
        return Database.GetInventory();
    }

    public void AddItemDatabase(ItemDatabase item){
        Database.AddItemDatabase(item);
        InventoryController.Instance.UpdateInventory();
    }

    public void RemoveItemDatabase(ItemDatabase item)
    {
        Database.RemoveItemDatabase(item);
    }

    public int GetInventoryCount()
    {
        return Database.ReturnSkillCount();
    }

    public void AddSkillFound(SkillDatabase item)
    {
        try
        {
            Database.AddSkill(item);
            SkillTreeController.Instance.VerifyButtons();
            UIController.Instance.UpdateHUD(scrolls: Database.ReturnSkillCount());
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    public void AddSkillLearned(SkillEnumerator item){
        Database.LearnSkill(item);
        SkillTreeController.Instance.VerifyButtons();
    }

    public bool VerifyItemFound(SkillEnumerator recipe){
        return Database.VerifySkillFound(recipe);
    }

    public bool VerifyItemLearned(SkillEnumerator recipe){
        return Database.VerifySkillLearned(recipe);
    }
}
