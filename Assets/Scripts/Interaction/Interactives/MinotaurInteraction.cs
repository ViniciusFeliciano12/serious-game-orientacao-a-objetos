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
                if (!GameController.Instance.DialogueAlreadyPlayed("MinotaurDeadDialogue"))
                {
                    DialogueManagement.Instance.StartDialogue("MinotaurDeadDialogue");
                    GameController.Instance.SaveDialoguePlayed("MinotaurDeadDialogue");
                }
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
                if (!GameController.Instance.DialogueAlreadyPlayed("FirstTorchLitDialogue"))
                {
                    DialogueManagement.Instance.StartDialogue("FirstTorchLitDialogue");
                    GameController.Instance.SaveDialoguePlayed("FirstTorchLitDialogue");
                }
            }

            if (quantity < 3 || quantity == 24)
            {
                GetComponent<AudioSource>().Play();
                WowCameraController.Instance.FocusOnMinotaur();
                animator.SetTrigger("TakeDamage");
            }

            if (quantity == 24)
            {
                if (!GameController.Instance.DialogueAlreadyPlayed("LastTorchDialogue"))
                {
                    DialogueManagement.Instance.StartDialogue("LastTorchDialogue");
                    GameController.Instance.SaveDialoguePlayed("LastTorchDialogue");
                }
            }

            UIController.Instance.SetTextTimeout($"Tocha acesa, faltam {24 - quantity}");
        }
    }
}
