using EJETAGame;
using UnityEngine;

public class RecipeInteraction : MonoBehaviour, IInteractable
{
    
    public KeyCode interactionKey;

    void Start(){
        
        
    }

    public void Interact()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            
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