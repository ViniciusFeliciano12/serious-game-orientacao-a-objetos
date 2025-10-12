
using EJETAGame;
using System.Collections.Generic;
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
        if (!(Input.GetKeyDown(interactionKey) && animator != null && !MainCharacterController.Instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Armed_Attack")))
        {
            return;
        }

        if (GameController.Instance.Database.ReturnSkillCount() < scrollsFoundToUnlock)
        {
            UIController.Instance.SetTextTimeout("Encontre todos os pergaminhos da sala para progredir");
            return;
        }

        if (cannotCloseAnymore)
        {
            UIController.Instance.SetTextTimeout("Porta destruída... não se pode mais fechar");
            return;
        }

        foreach (var key in keys)
        {
            if (key.skillID == GameDatabase.SkillEnumerator.Key)
            {
                if (InventoryController.Instance.VerifyItemSelected(key.skillID, key.propriedades, new() { isOpen ? key.metodos[1] : key.metodos[0] }))
                {
                    isOpen = !isOpen;
                    animator.SetBool("IsOpen", isOpen);
                    audioSources[0].Play();
                }
                else
                {
                    UIController.Instance.SetTextTimeout("Utilize a chave correta para interagir");
                }
            }
            else if (key.skillID == GameDatabase.SkillEnumerator.Crowbar)
            {
                if (InventoryController.Instance.VerifyItemSelected(key.skillID, metodos: key.metodos))
                {
                    if (!isBarredDoor)
                    {
                        cannotCloseAnymore = true;
                        isOpen = true;

                        MainCharacterController.Instance.animator.SetFloat("AttackSpeedMultiplier", 1.0f);
                        MainCharacterController.Instance.animator.SetTrigger("Attacking");

                        await Task.Delay(800);

                        animator.SetBool("IsOpen", isOpen);
                        audioSources[1].Play();
                        UIController.Instance.SetTextTimeout("Porta destruída... não se pode mais fechar");
                    }
                    else
                    {
                        UIController.Instance.SetTextTimeout("Só pode ser aberta pela chave correta");
                    }
                }
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
