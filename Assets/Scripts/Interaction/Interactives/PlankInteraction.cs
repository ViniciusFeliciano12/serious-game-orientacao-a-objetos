using EJETAGame;
using UnityEngine;

public class PlankInteraction : Interactable
{
    public override void Interact()
    {
        if (Input.GetKeyDown(interactionKey))
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
    }
}
