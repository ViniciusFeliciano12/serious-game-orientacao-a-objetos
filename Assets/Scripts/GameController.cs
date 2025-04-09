using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance { get; private set; }

    public GameDatabase Database; 

    private void Awake()
    {
        if(instance!= null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool VerifyRecipesUnlocked(Recipes recipe){
        return recipe switch
        {
            Recipes.Key => Database.recipeKeyLearned,
            Recipes.Sword => Database.recipeSwordLearned,
            _ => false,
        };
    }

    public enum Recipes{
        Key,
        Sword
    }
}
