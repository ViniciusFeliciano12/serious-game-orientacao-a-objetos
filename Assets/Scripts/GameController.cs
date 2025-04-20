using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        if(Instance!= null && Instance != this)
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

    public List<ItemDatabase> GetInventory(){
        return Database.GetInventory();
    }

    public void AddItemDatabase(ItemDatabase item){
        Database.AddItemDatabase(item);
        InventoryController.Instance.UpdateInventory();
    }

    public void AddSkillFound(SkillDatabase item){
        Database.AddSkill(item);
        SkillTreeController.Instance.VerifyButtons();
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
