using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using EJETAGame;
using UnityEngine;

public class CrateInteraction : Interactable
{
    void Start()
    {

    }

    public override async void Interact()
    {
        if (Input.GetKeyDown(interactionKey) && !MainCharacterController.Instance.CannotMove())
        {
            if (InventoryController.Instance.VerifyItemSelected(key.skillID, metodos: key.metodos))
            {
                MainCharacterController.Instance.animator.SetFloat("AttackSpeedMultiplier", 1.0f);
                MainCharacterController.Instance.animator.SetTrigger("Attacking");

                await Task.Delay(800);

                DespawnDebris();

                if (transform.TryGetComponent<AudioSource>(out AudioSource audio))
                {
                    audio.Play();
                }
            }
        }
    }

    public void DespawnDebris()
    {
        int despawnCount = transform.childCount * 60 / 100;

        for (int i = transform.childCount - 1; i >= transform.childCount - despawnCount; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        StartCoroutine(FadeAndDestroyRemaining(0.5f, 1f));
    }

    private IEnumerator FadeAndDestroyRemaining(float delay, float fadeDuration)
    {
        yield return new WaitForSeconds(delay);

        List<Renderer> renderers = new();

        foreach (Transform child in transform)
        {
            if (child.TryGetComponent<Renderer>(out var rend))
            {
                renderers.Add(rend);
            }
        }

        List<Material> mats = new();
        foreach (Renderer r in renderers)
        {
            mats.Add(r.material);
        }

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            foreach (Material m in mats)
            {
                if (m.HasProperty("_BaseColor"))
                {
                    Color c = m.color;
                    c.a = alpha;
                    m.color = c;
                }
            }

            yield return null;
        }

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        if (TryGetComponent<Collider>(out var col))
        {
            Destroy(col);
        }
    }

    public override void OnInteractEnter()
    {
        Debug.Log("interaction enter with crate");
        UIController.Instance.SetText("Utilize o pé de cabra para quebrar as caixas...");
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            Material highlightMat = new(Shader.Find("Universal Render Pipeline/Lit"));
            highlightMat.SetColor("_BaseColor", new Color(1f, 1f, 1f, 0.1f)); // branco transparente

            List<Material> mats = new(rend.materials)
            {
                highlightMat
            };
            rend.materials = mats.ToArray();
        }
    }

    public override void OnInteractExit()
    {
        Debug.Log("interaction exit with crate");
        UIController.Instance.SetText("");
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            List<Material> mats = new(rend.materials);

            // remove o último material (highlight)
            if (mats.Count > 1)
            {
                mats.RemoveAt(mats.Count - 1);
                rend.materials = mats.ToArray();
            }
        }
    }
}
