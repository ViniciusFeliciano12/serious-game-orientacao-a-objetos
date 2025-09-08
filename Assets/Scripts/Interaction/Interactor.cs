namespace EJETAGame
{
    using UnityEngine;

    public class Interactor : MonoBehaviour
    {
        private Transform interactorSource;

        [Header("Configurações de Interação")]
        [Tooltip("Define o tamanho total (largura, altura, profundidade) da caixa de detecção.")]
        [SerializeField] private Vector3 boxSize = new Vector3(1.5f, 1.5f, 1.5f);

        // NOVO: Campo para ajustar a posição do centro da caixa
        [Tooltip("Deslocamento do centro da caixa em relação ao pivô do personagem.")]
        [SerializeField] private Vector3 boxOffset = new Vector3(0f, 1f, 0f);

        [Tooltip("Define em qual camada (Layer) os objetos interativos se encontram.")]
        [SerializeField] private LayerMask interactableLayer;

        private IInteractable currentInteractable;
        public GameObject detectedObject;

        private void Awake()
        {
            interactorSource = transform;
        }

        private void Update()
        {
            // MODIFICADO: Adicionamos o 'boxOffset' à posição de origem
            Vector3 boxCenter = interactorSource.position + boxOffset;
            Collider[] hits = Physics.OverlapBox(boxCenter, boxSize / 2, interactorSource.rotation, interactableLayer);

            GameObject closestInteractableObject = null;
            float closestDistance = float.MaxValue;
            IInteractable foundInteractable = null;

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out IInteractable interactObj))
                {
                    // Usamos 'boxCenter' para o cálculo da distância para ser consistente
                    float distance = Vector3.Distance(boxCenter, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestInteractableObject = hit.gameObject;
                        foundInteractable = interactObj;
                    }
                }
            }

            // O resto da lógica permanece igual...
            if (foundInteractable != null)
            {
                detectedObject = closestInteractableObject;
                if (currentInteractable != foundInteractable)
                {
                    currentInteractable?.OnInteractExit();
                    foundInteractable.OnInteractEnter();
                    currentInteractable = foundInteractable;
                }
                foundInteractable.Interact();
                UIController.Instance.UpdateTextAppear(true);
            }
            else
            {
                if (currentInteractable != null)
                {
                    UIController.Instance.UpdateTextAppear(false);
                    currentInteractable.OnInteractExit();
                    currentInteractable = null;
                    detectedObject = null;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (interactorSource == null)
            {
                interactorSource = transform;
            }

            Gizmos.color = new Color(0, 1, 0, 0.3f);

            // MODIFICADO: Aplicamos o offset também na posição do Gizmo
            Vector3 gizmoCenter = interactorSource.position + boxOffset;
            Gizmos.matrix = Matrix4x4.TRS(gizmoCenter, interactorSource.rotation, Vector3.one);

            Gizmos.DrawCube(Vector3.zero, boxSize);
        }
    }
}