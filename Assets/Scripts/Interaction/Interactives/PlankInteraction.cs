using Assets.Scripts.Interaction.Interactives;
using EJETAGame;
using UnityEngine;

public class PlankInteraction : Interactable
{
    public override void Interact()
    {
        if (Input.GetKeyDown(interactionKey) && !DialogueManagement.Instance.HasActiveDialogue())
        {
            if (MinotaurInteraction.Instance.IsDead())
            {
                if (InventoryController.Instance.VerifyItemSelected(key.skillID, metodos: key.metodos))
                {
                    MainCharacterController.Instance.animator.SetFloat("AttackSpeedMultiplier", 1.0f);
                    MainCharacterController.Instance.animator.SetTrigger("Attacking");

                    if (TryGetComponent<Collider>(out var col))
                    {
                        col.enabled = false;
                    }
                    Destroy(gameObject, 2.5f);
                }
            }
            else
            {
                DialogueManagement.Instance.StartDialogue("TryBreakPlankWithoutMinotaurDeadDialogue");
            }
        }
    }
}
