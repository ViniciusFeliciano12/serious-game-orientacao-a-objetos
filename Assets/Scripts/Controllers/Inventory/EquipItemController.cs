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
    private Transform leftHandAttachPoint;

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
        leftHandAttachPoint = GameObject.Find("jointItemL").transform;
    }

#nullable enable

    private void Update()
    {
        if (ActualItem != null && ActualItem.skillID == SkillEnumerator.Torch)
        {
            ManageTorch();
        }

        if (ActualItem != null && (ActualItem.skillID != SkillEnumerator.Shield && ActualItem.skillID != SkillEnumerator.Set))
        {
            ManageShield();
        }
    }

    public ItemDatabase? ReturnAcutalItem()
    {
        return ActualItem;
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
        if (rightHandAttachPoint == null || leftHandAttachPoint == null)
        {
            Debug.LogError("O Ponto de Anexo (rightHandAttachPoint) ou (leftHandAttachPoint) não foi definido no Inspector!");
            return;
        }

        if (ActualItem.skillID == SkillEnumerator.Set)
        {
            ActualObject = Instantiate(Resources.Load<GameObject>("Escudo"), leftHandAttachPoint);
            ActualObject = Instantiate(Resources.Load<GameObject>("Espada"), rightHandAttachPoint);
            return;
        }

        var prefab = Resources.Load<GameObject>(ActualItem.nome);

        if (prefab == null)
        {
            return;
        }

        if (ActualItem.skillID == SkillEnumerator.Shield)
        {
            ActualObject = Instantiate(prefab, leftHandAttachPoint);
        }
        else
        {
            ActualObject = Instantiate(prefab, rightHandAttachPoint);
        }
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

    private void ManageShield()
    {
        if (MainCharacterController.Instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Blocking"))
        {
            MainCharacterController.Instance.animator.SetBool("Blocking", false);
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

        if (leftHandAttachPoint != null)
        {
            foreach (Transform child in leftHandAttachPoint)
            {
                Destroy(child.gameObject);
            }
        }

        ActualItem = null;
        ActualObject = null;
    }
}