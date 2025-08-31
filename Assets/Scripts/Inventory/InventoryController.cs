using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameDatabase;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance { get; private set; }

    public ItemInventoryController[] Inventory;
    private int indexSelected = -1;

    private Dictionary<SkillEnumerator, System.Action> itemUseActions;

    private void Awake()
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

    void Start()
    {
        var menu = GameObject.Find("Bottom itens");
        Inventory = menu.GetComponentsInChildren<ItemInventoryController>();

        InitializeItemActions(); 

        UpdateInventory();
    }

    private void InitializeItemActions()
    {
        itemUseActions = new Dictionary<SkillEnumerator, System.Action>
        {
             { SkillEnumerator.Gravel, UseGravel },
        };
    }

    void Update()
    {
        SelectItemInventory();

        if (Input.GetKeyDown(KeyCode.R))
        {
            RemoveItemInventory();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            TryUseSelectedItem();
        }
    }

    public bool VerifyItemSelected(SkillEnumerator skillID, List<StringPair> propriedades = null, List<StringPair> metodos = null)
    {
        if (indexSelected != -1)
        {
            var actualItem = Inventory[indexSelected].returnActualItem();
            if (actualItem != null)
            {
                bool idMatches = actualItem.skillID == skillID;
                bool propriedadesMatch = propriedades == null || List1IsContainedInList2(propriedades, actualItem.propriedades);
                bool metodosMatch = metodos == null || List1IsContainedInList2(metodos, actualItem.metodos);
                if (idMatches && propriedadesMatch && metodosMatch)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool List1IsContainedInList2(List<StringPair> list1, List<StringPair> list2)
    {
        return !list1.Except(list2, new StringPairComparer()).Any();
    }

    private void RemoveItemInventory()
    {
        if (indexSelected != -1)
        {
            var item = Inventory[indexSelected].returnActualItem();
            if (item != null)
            {
                GameController.Instance.RemoveItemDatabase(item);
                Inventory[indexSelected].ResetDefaults();
            }
        }
    }

    private void TryUseSelectedItem()
    {
        if (indexSelected != -1)
        {
            var item = Inventory[indexSelected].returnActualItem();
            if (item != null)
            {
                if (itemUseActions.TryGetValue(item.skillID, out System.Action useAction))
                {
                    useAction.Invoke();
                }
            }
        }
    }

    public void UpdateInventory()
    {
        var inventoryDatabase = GameController.Instance.GetInventory();
        for (int i = 0; i < inventoryDatabase.Count; i++)
        {
            Inventory[i].InstantiateItem(inventoryDatabase[i]);
        }
    }

    private void SelectItemInventory()
    {
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + i))
            {
                SelectItemAt(i - 1);
                return;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        {
            SelectItemAt(9);
        }
    }

    public void SelectItemAt(int index)
    {
        if (index >= Inventory.Length) return; 

        indexSelected = index;
        for (int i = 0; i < Inventory.Length; i++)
        {
            Inventory[i].ItemSelected(i == index);
        }
    }

    #region Item Use Functions

    private void UseGravel() 
    {
        Debug.Log("usando gravel...");
    }

    #endregion
}