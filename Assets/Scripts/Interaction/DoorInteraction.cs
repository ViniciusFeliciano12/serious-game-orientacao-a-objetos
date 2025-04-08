

namespace EJETAGame
{
    using UnityEngine;

    public class InteractionTEST : MonoBehaviour, IInteractable
    {
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
                isOpen = !isOpen;
                animator.SetBool("IsOpen", isOpen);
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
