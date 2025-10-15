using EJETAGame;
using UnityEngine;

namespace Assets.Scripts.Interaction.Interactives
{
    public class MinotaurInteraction : EnemyInteraction
    {
        public static MinotaurInteraction Instance { get; private set; }

        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        public override void ReceberDano(double quantidade)
        {
            base.ReceberDano(quantidade);

            if (estadoAtual == EstadoIA.Morto)
            {
                DialogueManagement.Instance.StartDialogueOnlyOnce("MinotaurDeadDialogue");
            }
        }

        public bool IsDead()
        {
            return estadoAtual == EstadoIA.Morto;
        }

        public void ReceberDanoFromTorch(double quantidade)
        {
            base.ReceberDano(quantidade);

            if (animator != null)
            {
                animator = GetComponent<Animator>();
            }

            var quantity = GameController.Instance.QuantityTorchesLit();

            if (quantity == 1)
            {
                DialogueManagement.Instance.StartDialogueOnlyOnce("FirstTorchLitDialogue");
            }

            if (quantity < 3 || quantity == 24)
            {
                GetComponent<AudioSource>().Play();
                WowCameraController.Instance.FocusOnMinotaur();
                animator.SetTrigger("TakeDamage");
            }

            if (quantity == 24)
            {
                DialogueManagement.Instance.StartDialogueOnlyOnce("LastTorchDialogue");
            }

            UIController.Instance.SetTextTimeout($"Tocha acesa, faltam {24 - quantity}");
        }
    }
}
