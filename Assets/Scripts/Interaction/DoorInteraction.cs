

namespace EJETAGame
{
    using System.Collections.Generic;
    using UnityEngine;

    public class DoorInteraction : MonoBehaviour, IInteractable
    {
        public List<ItemDatabase> keys;
        public Animator animator { get; set; }
        private bool isOpen = false;
        private bool cannotCloseAnymore = false;

        void Start(){
            animator = transform.parent.GetComponent<Animator>();
            
            animator.SetBool("IsOpen", isOpen);
        }

        //Which button the user must press to initiate the Interaction;
        public KeyCode interactionKey;
        public void Interact()
        {
            if (!cannotCloseAnymore)
            {
                if (Input.GetKeyDown(interactionKey) && animator != null)
                {
                    foreach (var key in keys)
                    {
                        if (key.skillID == GameDatabase.SkillEnumerator.Key)
                        {
                            if (InventoryController.Instance.VerifyItemSelected(key.skillID, key.propriedades, new() { isOpen ? key.metodos[1] : key.metodos[0] }))
                            { //verificar se pode interagir
                                isOpen = !isOpen;
                                animator.SetBool("IsOpen", isOpen);
                            }
                            else
                            {
                                InteractionText.instance.SetTextTimeout("Utilize a chave correta para interagir");
                            }
                        }
                        else if (key.skillID == GameDatabase.SkillEnumerator.Crowbar)
                        {
                            if (InventoryController.Instance.VerifyItemSelected(key.skillID, metodos: key.metodos))
                            {
                                cannotCloseAnymore = true;
                                isOpen = true;
                                animator.SetBool("IsOpen", isOpen);
                                InteractionText.instance.SetTextTimeout("Porta destruída... não se pode mais fechar");
                            }
                        }
                    }
                }
            }
            else
            {
                InteractionText.instance.SetTextTimeout("Porta destruída... não se pode mais fechar");
            }
        }

        public void OnInteractEnter()
        {
            if (!cannotCloseAnymore)
            {
                InteractionText.instance.SetText("Aperte " + interactionKey + " para abrir ou fechar a porta");
            }
        }


        public void OnInteractExit()
        {
            Debug.Log("Interaction Ended");
        }
    }

}
