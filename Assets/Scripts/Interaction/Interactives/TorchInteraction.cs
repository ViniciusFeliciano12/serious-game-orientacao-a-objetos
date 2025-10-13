using Assets.Scripts.Interaction.Interactives;
using EJETAGame;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TorchInteraction : Interactable
{
    private GameObject Smoke;
    private GameObject Fire;
    private Renderer[] torchRenderers;
    private bool active;

    public int torchId;
    
    void Start()
    {
        Smoke = transform.Find("Smoke").gameObject;
        Fire = transform.Find("Fire").gameObject;

        if (GameController.Instance.TorchAlreadyLit(torchId))
        {
            ActiveTorch();
            MinotaurInteraction.Instance.ReceberDano(4);
        }
    }

    public override void Interact()
    {
        if (Input.GetKeyDown(interactionKey) && !active)
        {
            if (key.skillID == GameDatabase.SkillEnumerator.Ignitor)
            {
                if (InventoryController.Instance.VerifyItemSelected(key.skillID))
                {
                    ActiveTorch();
                    GameController.Instance.SaveTorchLit(torchId);
                    MinotaurInteraction.Instance.ReceberDanoFromTorch(4);
                }
            }
        }
    }

    private void ActiveTorch()
    {
        active = true;

        Smoke.SetActive(active);
        Fire.SetActive(active);
    }

    public override void OnInteractEnter()
    {
        Transform torchModelParent = transform.Find("Torch");

        if (torchModelParent != null)
        {
            torchRenderers = torchModelParent.GetComponentsInChildren<Renderer>();

            Material highlightMat = new(Shader.Find("Universal Render Pipeline/Lit"));
            highlightMat.SetColor("_BaseColor", new Color(1f, 1f, 1f, 0.3f)); // Branco semitransparente

            foreach (var rend in torchRenderers)
            {
                List<Material> mats = new(rend.materials)
                {
                    highlightMat
                };
                rend.materials = mats.ToArray();
            }
        }
    }

    public override void OnInteractExit()
    {
        if (torchRenderers != null)
        {
            foreach (var rend in torchRenderers)
            {
                List<Material> mats = new(rend.materials);
                if (mats.Count > 0)
                {
                    mats.RemoveAt(mats.Count - 1);
                    rend.materials = mats.ToArray();
                }
            }
        }
    }

}
