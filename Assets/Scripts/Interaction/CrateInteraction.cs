

namespace EJETAGame
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CrateInteraction : MonoBehaviour, IInteractable
    {
        public ItemDatabase key;
        void Start()
        {
           
        }

        //Which button the user must press to initiate the Interaction;
        public KeyCode interactionKey;
        public void Interact()
        {
            if (Input.GetKeyDown(interactionKey))
            {
                if (key.skillID == GameDatabase.SkillEnumerator.Crowbar)
                {
                    if (InventoryController.Instance.VerifyItemSelected(key.skillID, metodos: key.metodos))
                    {
                        DespawnDebris();
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

            // pega todos os filhos que sobraram
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<Renderer>(out var rend))
                {
                    renderers.Add(rend);
                }
            }

            // guarda os materiais
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


        public void OnInteractEnter()
        {
            Debug.Log("interaction enter with crate");
            InteractionText.instance.SetText("Utilize o pé de cabra para quebrar as caixas...");
            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            foreach (Renderer rend in renderers)
            {
                // Cria um material URP Lit para highlight
                Material highlightMat = new(Shader.Find("Universal Render Pipeline/Lit"));
                highlightMat.SetColor("_BaseColor", new Color(1f, 1f, 1f, 0.1f)); // branco transparente

                // Adiciona o material no objeto filho
                List<Material> mats = new(rend.materials)
                {
                    highlightMat
                };
                rend.materials = mats.ToArray();
            }
        }

        public void OnInteractExit()
        {
            Debug.Log("interaction exit with crate");
            InteractionText.instance.SetText("");
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

}
