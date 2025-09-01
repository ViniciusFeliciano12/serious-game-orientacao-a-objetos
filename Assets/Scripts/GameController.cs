using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameDatabase;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public GameDatabase Database; 
    private bool PlayerCanMove = true;
    private void Awake()
    {
        // Database.ResetDatabase();
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void AddSkillFound(SkillDatabase item)
    {
        try
        {
            Database.AddSkill(item);
            SkillTreeController.Instance.VerifyButtons();
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
