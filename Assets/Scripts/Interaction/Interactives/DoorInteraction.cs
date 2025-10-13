
using EJETAGame;
using Esper.FeelSpeak;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class DoorInteraction : Interactable
{
    public List<ItemDatabase> keys;
    public Animator animator { get; set; }
    public bool isBarredDoor = false;
    public bool lastDoor = false;
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
        if (!Input.GetKeyDown(interactionKey) || animator == null || MainCharacterController.Instance.CannotMove())
        {
            return;
        }

        if (GameController.Instance.GetInventoryLearnedCount() < scrollsFoundToUnlock)
        {
            DialogueManagement.Instance.StartDialogue("LearnAllRecipesDialogue");
            return;
        }

        var actualItem = InventoryController.Instance.ReturnActualItem();

        var key = keys.FirstOrDefault(item => item.skillID == actualItem.skillID);

        if (actualItem.skillID == GameDatabase.SkillEnumerator.Key)
        {
            if (cannotCloseAnymore)
            {
                DialogueManagement.Instance.StartDialogue("DoorBrokeDialogue");
                return;
            }

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
            if (cannotCloseAnymore)
            {
                DialogueManagement.Instance.StartDialogue("DoorBrokeDialogue");
                return;
            }

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

            base.Interact();

            if (lastDoor && isOpen)
            {
                PauseController.Instance.FinishGame();
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
