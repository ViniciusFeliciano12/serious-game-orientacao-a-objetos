namespace EJETAGame
{
    using UnityEngine;

    public class Interactor : MonoBehaviour
    {
        Transform interactorSource; // Centro da esfera de detecção
        [SerializeField] float interactRadius = 1.5f; // Raio da esfera de detecção

        private IInteractable currentInteractable; // Objeto interativo atual
        public GameObject detectedObject;

        private void Awake()
        {
            interactorSource = transform;
        }

        private void Update()
        {
            Collider[] hits = Physics.OverlapSphere(interactorSource.position, interactRadius);

            GameObject closestInteractableObject = null;
            float closestDistance = float.MaxValue;
            IInteractable foundInteractable = null;

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out IInteractable interactObj))
                {
                    float distance = Vector3.Distance(interactorSource.position, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestInteractableObject = hit.gameObject;
                        foundInteractable = interactObj;
                    }
                }
            }

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
                InteractionText.instance.textAppear.gameObject.SetActive(true);
            }
            else
            {
                if (currentInteractable != null)
                {
                    InteractionText.instance.textAppear.gameObject.SetActive(false);
                    currentInteractable.OnInteractExit();
                    currentInteractable = null;
                }
            }
        }
    }
}
