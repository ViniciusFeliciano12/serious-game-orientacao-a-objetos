

namespace EJETAGame
{
    using System.Collections.Generic;
    using UnityEngine;

    public class DoorInteraction : MonoBehaviour, IInteractable
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

        //Which button the user must press to initiate the Interaction;
        public KeyCode interactionKey;
        public void Interact()
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

        public void OnInteractEnter()
        {
            Debug.Log("interaction enter with door");

            if (!cannotCloseAnymore)
            {
                InteractionText.instance.SetText("Clique com o mouse para abrir ou fechar a porta");
            }

            if (TryGetComponent<Renderer>(out var rend))
            {
                Material highlightMat = new(Shader.Find("Universal Render Pipeline/Lit"));
                highlightMat.SetColor("_BaseColor", new Color(1f, 1f, 1f, 0.3f)); // branco transparente

                List<Material> mats = new(rend.materials)
                {
                    highlightMat
                };

                rend.materials = mats.ToArray();
            }
        }

        public void OnInteractExit()
        {
            Debug.Log("interaction exit with door");

            InteractionText.instance.SetText("");
            if (TryGetComponent<Renderer>(out var rend))
            {
                // Remove o último material (highlight)
                List<Material> mats = new List<Material>(rend.materials);
                if (mats.Count > 1)
                {
                    mats.RemoveAt(mats.Count - 1);
                    rend.materials = mats.ToArray();
                }
            }
        }
    }

}
