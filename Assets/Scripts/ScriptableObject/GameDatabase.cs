using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "Scriptables/Database", order = 1)]
public class GameDatabase : ScriptableObject
{
    public bool recipeKeyLearned;
    public bool recipeSwordLearned;

    public void ResetDatabase(){
        recipeKeyLearned = false;
        recipeSwordLearned = false;
    }
}