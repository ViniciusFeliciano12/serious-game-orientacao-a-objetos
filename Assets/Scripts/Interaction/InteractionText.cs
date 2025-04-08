
namespace EJETAGame
{
    using TMPro;
    using Unity.VisualScripting;
    using UnityEngine;

    public class InteractionText : MonoBehaviour
    {
        public static InteractionText instance { get; private set; }

        public TextMeshProUGUI textAppear;
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

    }

}