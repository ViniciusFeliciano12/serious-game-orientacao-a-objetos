using EJETAGame;
using UnityEngine;
using static GameDatabase;

public class RecipeInteraction : Interactable
{
    public SkillEnumerator itemObject;

    void Start(){
        
        if(GameController.Instance.VerifyItemFound(itemObject)){
            Destroy(gameObject);
        }
    }

    public override void Interact()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            SkillDatabase RecipeItem = new(){
                itemID = itemObject,
                recipeLearned = false
            };

            GameController.Instance.AddSkillFound(RecipeItem);
            InteractionText.instance.SetTextTimeout("Pegou receita nova! Aperte I para verificar sua árvore de habilidades");
            CharacterController.Instance.animator.SetTrigger("Gathering");

            if (TryGetComponent<Collider>(out var col))
            {
                Destroy(col);
            }

            Destroy(gameObject, 0.2f);
        }
    }

    public override void OnInteractEnter()
    {
        InteractionText.instance.SetText("Clique com o mouse para pegar a receita");

        base.OnInteractEnter();
    }
}