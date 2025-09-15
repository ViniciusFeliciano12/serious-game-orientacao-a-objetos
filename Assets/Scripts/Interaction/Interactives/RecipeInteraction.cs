using EJETAGame;
using UnityEngine;
using static GameDatabase;

public class RecipeInteraction : Interactable
{
    public SkillEnumerator itemObject;

    public bool isntRecipe;

    void Start(){
        
        if(GameController.Instance.VerifyItemFound(itemObject)){
            Destroy(gameObject);
        }
    }

    public override void Interact()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            SkillDatabase RecipeItem = new()
            {
                itemID = itemObject,
                recipeLearned = false
            };

            GameController.Instance.AddSkillFound(RecipeItem);

            if (!isntRecipe)
            {
                UIController.Instance.SetTextTimeout("Pegou receita nova! Aperte I para verificar sua árvore de habilidades");
            }
            else
            {
                UIController.Instance.SetTextTimeout("Aprendeu a reutilização dos itens que você possui! Aperte E para remodelar ele.");
            }

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
        UIController.Instance.SetText("Clique com o mouse para pegar a receita");

        base.OnInteractEnter();
    }
}