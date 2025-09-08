using UnityEngine;
// Adicionei esta linha para o Tooltip, é uma boa prática
using static GameDatabase;

public class EquipItemController : MonoBehaviour
{
    public static EquipItemController Instance { get; private set; }

#nullable enable
    private ItemDatabase? ActualItem { get; set; }
    private GameObject? ActualObject { get; set; }
#nullable disable

    private Transform rightHandAttachPoint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        rightHandAttachPoint = GameObject.Find("jointItemR").transform;
    }

#nullable enable

    private void Update()
    {
        if (ActualItem != null && ActualItem.skillID == SkillEnumerator.Torch)
        {
            ManageTorch();
        }
    }

    public void EquipItem(ItemDatabase? item)
    {
        if (ActualItem == item) return;

        Desequip();
        ActualItem = item;

        if (item != null)
        {
            EquipItem();
        }
    }
#nullable disable

    private void EquipItem()
    {
        if (rightHandAttachPoint == null)
        {
            Debug.LogError("O Ponto de Anexo (rightHandAttachPoint) não foi definido no Inspector!");
            return;
        }

        var prefab = Resources.Load<GameObject>(ActualItem.nome);

        if (prefab == null)
        {
            return;
        }

        ActualObject = Instantiate(prefab, rightHandAttachPoint);
    }

    private void ManageTorch()
    {
        Transform smokeTransform = ActualObject.transform.Find("Smoke");
        Transform fireTransform = ActualObject.transform.Find("Fire");

        if (smokeTransform != null && fireTransform != null)
        {
            GameObject smokeObject = smokeTransform.gameObject;
            GameObject fireObject = fireTransform.gameObject;

            smokeObject.SetActive(ActualItem.itemActive);
            fireObject.SetActive(ActualItem.itemActive);
        }     
    }

    private void Desequip()
    {
        if (rightHandAttachPoint != null)
        {
            foreach (Transform child in rightHandAttachPoint)
            {
                Destroy(child.gameObject);
            }
        }

        ActualItem = null;
        ActualObject = null;
    }
}