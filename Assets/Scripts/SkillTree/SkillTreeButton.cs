using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static GameDatabase;

public class SkillTreeButton : MonoBehaviour
{
    public string title;
    public SkillEnumerator itemID;
    public List<Item> propriedades;
    public List<Item> metodos;
    public List<string> palavras;


    private void Start()
    {
        
    }

    public void Clicked(){
        if(!GameController.Instance.VerifyItemLearned(itemID)){
            LearnNewRecipeMinigameController.Instance.StartNewGame(title, itemID, palavras, propriedades, metodos);
        }
        else{
            LearnNewRecipeMinigameController.Instance.CraftItem(title, itemID, propriedades, metodos);
        }
    }
}
