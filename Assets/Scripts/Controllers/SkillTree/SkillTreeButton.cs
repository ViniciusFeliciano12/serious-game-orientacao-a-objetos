using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static GameDatabase;
using UnityEngine.UI;
using EJETAGame;

public class SkillTreeButton : MonoBehaviour
{
    public string title;
    public SkillEnumerator itemID;
    public SkillEnumerator[] dependsOnAncestorItem;
    public bool isInterface;
    public List<Item> propriedades;
    public List<Item> metodos;
    public List<string> palavras;

    private Transform xMark; // Guardar referência pro XMark

    private void Start()
    {
        xMark = transform.Find("XMark"); // Já pega o XMark no Start
    }

    public void Clicked()
    {
        // Se o XMark estiver ativo, não deixa clicar
        if (xMark != null && xMark.gameObject.activeSelf)
        {
            // Só pra feedback se quiser: Debug.Log("Não pode clicar ainda! Depêndencias não aprendidas.");
            return;
        }

        // Caso o item ainda não tenha sido aprendido, inicia o minigame
        if (!GameController.Instance.VerifyItemLearned(itemID))
        {
            StartNewRecipeMinigame();
            return;
        }

        // Caso o item já tenha sido aprendido, processa de acordo com o tipo de item
        if (isInterface)
        {
            HandleInterfaceItem();
        }
        else
        {
            StartCraftItemMinigame();
        }
    }

    private void StartNewRecipeMinigame()
    {
        LearnNewRecipeMinigameController.Instance.StartNewGame(title, itemID, palavras, propriedades, metodos, isInterface);
    }

    private void HandleInterfaceItem()
    {
        LearnNewRecipeMinigameController.Instance.ClearFields();
        UIController.Instance.SetTextTimeout("Classes abstratas não podem ser instanciadas");
    }

    private void StartCraftItemMinigame()
    {
        var texture = transform.Find("Icon").GetComponent<RawImage>().texture;
        LearnNewRecipeMinigameController.Instance.CraftItem(title, itemID, propriedades, metodos, texture);
    }
}
