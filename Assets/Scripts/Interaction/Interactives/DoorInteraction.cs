
using EJETAGame;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class DoorInteraction : Interactable
{
    public List<ItemDatabase> keys;
    public Animator animator { get; set; }
    public bool isBarredDoor = false;
    public int scrollsFoundToUnlock = -1;
    private bool isOpen = false;
    private bool cannotCloseAnymore = false;
    private AudioSource[] audioSources;

    void Start(){
        animator = transform.parent.GetComponent<Animator>();
        audioSources = transform.GetComponents<AudioSource>();

        animator.SetBool("IsOpen", isOpen);
    }
       
    public override async void Interact()
    {
        if (!(Input.GetKeyDown(interactionKey) && !DialogueManagement.Instance.HasActiveDialogue() && animator != null && !MainCharacterController.Instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Armed_Attack")))
        {
            return;
        }

        if (GameController.Instance.Database.ReturnSkillLearnedCount() < scrollsFoundToUnlock)
        {
            DialogueManagement.Instance.StartDialogue("LearnAllRecipesDialogue");
            return;
        }

        if (cannotCloseAnymore)
        {
            DialogueManagement.Instance.StartDialogue("DoorBrokeDialogue");
            return;
        }

        var actualItem = InventoryController.Instance.ReturnActualItem();

        var key = keys.FirstOrDefault(item => item.skillID == actualItem.skillID);

        if (actualItem.skillID == GameDatabase.SkillEnumerator.Key)
        {
            if (InventoryController.Instance.VerifyItemSelected(key.skillID, key.propriedades, new() { isOpen ? key.metodos[1] : key.metodos[0] }))
            {
                isOpen = !isOpen;
                animator.SetBool("IsOpen", isOpen);
                audioSources[0].Play();
            }
            else
            {
                DialogueManagement.Instance.StartDialogue("WrongDoorDialogue");
            }
        }

        if (actualItem.skillID == GameDatabase.SkillEnumerator.Crowbar)
        {
            MainCharacterController.Instance.animator.SetFloat("AttackSpeedMultiplier", 1.0f);
            MainCharacterController.Instance.animator.SetTrigger("Attacking");

            await Task.Delay(800);

            if (!isBarredDoor)
            {
                cannotCloseAnymore = true;
                isOpen = true;

                animator.SetBool("IsOpen", isOpen);
                audioSources[1].Play();
                UIController.Instance.SetTextTimeout("Porta destruída... não se pode mais fechar");
            }
            else
            {
                DialogueManagement.Instance.StartDialogue("OnlyOpenByKeyDialogue");
            }
        }
    }

    public override void OnInteractEnter()
    {
        Debug.Log("interaction enter with door");

        if (!cannotCloseAnymore)
        {
            UIController.Instance.SetText("Clique com o mouse para abrir ou fechar a porta");
        }

        base.OnInteractEnter();
    }
}
