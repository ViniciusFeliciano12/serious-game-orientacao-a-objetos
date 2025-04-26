

namespace EJETAGame
{
    using System.Collections.Generic;
    using UnityEngine;

    public class InteractionTEST : MonoBehaviour, IInteractable
    {
        public ItemDatabase key;
        public Animator animator { get; set; }
        private bool isOpen = false;
        

        void Start(){
            animator = transform.parent.GetComponent<Animator>();
            
            animator.SetBool("IsOpen", isOpen);
        }

        //Which button the user must press to initiate the Interaction;
        public KeyCode interactionKey;
        public void Interact()
        {
            if (Input.GetKeyDown(interactionKey) && animator != null)
            {
                if(InventoryController.Instance.VerifyItemSelected(key.skillID, key.propriedades, new() { isOpen ? key.metodos[1] : key.metodos[0]})){ //verificar se pode interagir
                    isOpen = !isOpen;
                    animator.SetBool("IsOpen", isOpen);
                } else{
                    InteractionText.instance.SetTextTimeout("Utilize a chave correta para interagir");
                }
            }
        }

        public void OnInteractEnter()
        {
            InteractionText.instance.SetText("Aperte "+interactionKey+" para abrir ou fechar a porta");
        }


        public void OnInteractExit()
        {
            Debug.Log("Interaction Ended");
        }
    }

}
