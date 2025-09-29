using EJETAGame;

namespace Assets.Scripts.Interaction.Interactives
{
    public class MinotaurInteraction : EnemyInteraction
    {
        public static MinotaurInteraction Instance { get; private set; }

        public override void Start()
        {
            base.Start();

            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }  
    }
}
