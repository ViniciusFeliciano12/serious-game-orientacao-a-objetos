using EJETAGame;
using Unity.VisualScripting;
using UnityEngine;
using static GameDatabase;

public class RecipeInteraction : MonoBehaviour, IInteractable
{
    public KeyCode interactionKey;
    public ItemEnumerator itemObject;

    void Start(){
        
        if(GameController.Instance.VerifyItemFound(itemObject)){
            Destroy(gameObject);
        }
    }

    public void Interact()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            ItemDatabase RecipeItem = new(){
                itemID = itemObject,
                recipeLearned = false
            };

            GameController.Instance.AddItemFound(RecipeItem);
            InteractionText.instance.SetTextTimeout("Pegou receita nova! Aperte I para verificar sua árvore de habilidades");
            Destroy(gameObject);
        }
    }

    public void OnInteractEnter()
    {
        InteractionText.instance.SetText("Aperte " +interactionKey+ " para pegar a receita");
    }


    public void OnInteractExit()
    {
        Debug.Log("Interaction Ended");
    }
}