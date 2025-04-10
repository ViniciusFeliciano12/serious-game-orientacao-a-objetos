
namespace EJETAGame
{
    using System.Collections;
    using TMPro;
    using Unity.VisualScripting;
    using UnityEngine;

    public class InteractionText : MonoBehaviour
    {
        public static InteractionText instance { get; private set; }

        public TextMeshProUGUI textAppear;
        public TextMeshProUGUI textAppearWithTimeout;
        private void Awake()
        {
            if(instance!= null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }

        public void SetText(string text)
        {
            textAppear.SetText(text);
        }

        public void SetTextTimeout(string text)
        {
            StopAllCoroutines(); // Garante que qualquer texto anterior seja interrompido
            StartCoroutine(FadeTextRoutine(text));
        }

        private IEnumerator FadeTextRoutine(string text)
        {
            textAppearWithTimeout.SetText(text);
            Color originalColor = textAppearWithTimeout.color;

            // Fade-in
            float timer = 0f;
            while (timer < 0.5)
            {
                float alpha = Mathf.Lerp(0f, 1f, timer / 0.5f);
                textAppearWithTimeout.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                timer += Time.deltaTime;
                yield return null;
            }
            textAppearWithTimeout.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

            // Espera 2 segundos com o texto visível
            yield return new WaitForSeconds(2f);

            // Fade-out
            timer = 0f;
            while (timer < 0.5f)
            {
                float alpha = Mathf.Lerp(1f, 0f, timer / 0.5f);
                textAppearWithTimeout.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                timer += Time.deltaTime;
                yield return null;
            }
            textAppearWithTimeout.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        }
    }

}