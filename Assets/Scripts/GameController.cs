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
        Database.ResetDatabase();

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

    public void AddItemFound(ItemDatabase item){
        Database.AddItem(item);
        SkillTreeController.Instance.VerifyButtons();
    }

    public void AddItemLearned(ItemEnumerator item){
        Database.LearnItem(item);
        SkillTreeController.Instance.VerifyButtons();
    }

    public bool VerifyItemFound(ItemEnumerator recipe){
        return Database.VerifyItemFound(recipe);
    }

    public bool VerifyItemLearned(ItemEnumerator recipe){
        return Database.VerifyItemLearned(recipe);
    }
}
