using System.Collections.Generic;
using EJETAGame;
using Unity.VisualScripting;
using UnityEngine;
using static GameDatabase;

public class RecipeInteraction : MonoBehaviour, IInteractable
{
    public KeyCode interactionKey;
    public SkillEnumerator itemObject;

    void Start(){
        
        if(GameController.Instance.VerifyItemFound(itemObject)){
            Destroy(gameObject);
        }
    }

    public void Interact()
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
            Destroy(gameObject);
        }
    }

    public void OnInteractEnter()
    {
        InteractionText.instance.SetText("Aperte " + interactionKey + " para pegar a receita");

        if (TryGetComponent<Renderer>(out var rend))
        {
            // Cria material temporário com URP Lit
            Material highlightMat = new(Shader.Find("Universal Render Pipeline/Lit"));
            highlightMat.SetColor("_BaseColor", new Color(1f, 1f, 1f, 0.3f)); // branco transparente

            // Garante que não duplica
            List<Material> mats = new(rend.materials)
            {
                highlightMat
            };
            rend.materials = mats.ToArray();
        }
    }


    public void OnInteractExit()
    {
        InteractionText.instance.SetText("");
        Debug.Log("Interaction Ended");
    }
}