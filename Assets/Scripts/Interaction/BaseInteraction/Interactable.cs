namespace EJETAGame
{
    using Esper.FeelSpeak;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class Interactable : MonoBehaviour, IInteractable
    {
        //Which button the user must press to initiate the Interaction;
        public KeyCode interactionKey;
        public ItemDatabase key;
        public string dialogueName;

        void Start(){
            
        }

        public virtual void Interact()
        {
            if (Input.GetKeyDown(interactionKey) && !DialogueManagement.Instance.HasActiveDialogue() && !string.IsNullOrEmpty(dialogueName))
            {
                if (!GameController.Instance.DialogueAlreadyPlayed(dialogueName))
                { 
                    DialogueManagement.Instance.StartDialogue(dialogueName);
                    GameController.Instance.SaveDialoguePlayed(dialogueName);
                }
            }
        }

        public virtual void OnInteractEnter()
        {
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

        public virtual void OnInteractExit()
        {
            UIController.Instance.SetText("");
            if (TryGetComponent<Renderer>(out var rend))
            {
                // Remove o último material (highlight)
                List<Material> mats = new(rend.materials);
                if (mats.Count > 1)
                {
                    mats.RemoveAt(mats.Count - 1);
                    rend.materials = mats.ToArray();
                }
            }
        }
    }

}
