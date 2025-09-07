namespace EJETAGame
{
    using System.Collections.Generic;
    using UnityEngine;

    public class DoorInteraction : Interactable
    {
        public List<ItemDatabase> keys;
        public Animator animator { get; set; }
        public bool isBarredDoor = false;
        private bool isOpen = false;
        private bool cannotCloseAnymore = false;

        void Start(){
            animator = transform.parent.GetComponent<Animator>();
            
            animator.SetBool("IsOpen", isOpen);
        }
       
        public override void Interact()
        {
            if (Input.GetKeyDown(interactionKey) && animator != null)
            {
                if (!cannotCloseAnymore)
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
                                if (!isBarredDoor)
                                {
                                    cannotCloseAnymore = true;
                                    isOpen = true;
                                    animator.SetBool("IsOpen", isOpen);
                                    InteractionText.instance.SetTextTimeout("Porta destruída... não se pode mais fechar");
                                }
                                else
                                {
                                    InteractionText.instance.SetTextTimeout("Só pode ser aberta pela chave correta");
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
        }

        public override void OnInteractEnter()
        {
            Debug.Log("interaction enter with door");

            if (!cannotCloseAnymore)
            {
                InteractionText.instance.SetText("Clique com o mouse para abrir ou fechar a porta");
            }

            base.OnInteractEnter();
        }
    }

}
