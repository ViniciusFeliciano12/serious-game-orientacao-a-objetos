using System.Collections.Generic;
using System.Linq;
using EJETAGame;
using UnityEngine;
using static GameDatabase;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance { get; private set; }

    public ItemInventoryController[] Inventory;
    private int indexSelected = -1;

    private Dictionary<SkillEnumerator, System.Action> itemUseActions;

    private bool usingGravel = false;

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
             { SkillEnumerator.Torch, UseTorch },
             { SkillEnumerator.Crowbar, UseCrowbar }
        };
    }

    void Update()
    {
        SelectItemInventory();

        TryUseSelectedActiveItem();

        if (Input.GetKeyDown(KeyCode.R) && Inventory[indexSelected].returnActualItem() != null)
        {
            FindInactive.Find("ConfirmErase").SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            TryUseSelectedItem();
        }
    }

    private void TryUseSelectedActiveItem()
    {
        if (indexSelected != -1)
        {
            for (int i = 0; i < Inventory.Length; i++)
            {
                Inventory[i].VerifyItemActive(i == indexSelected);
            }
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

    public void RemoveItemInventory()
    {
        if (indexSelected != -1)
        {
            var item = Inventory[indexSelected].returnActualItem();
            if (item != null)
            {
                GameController.Instance.RemoveItemDatabase(item);
                Inventory[indexSelected].ResetDefaults();
            }

            CloseConfirmRemoveItem();
        }
    }

    public void CloseConfirmRemoveItem()
    {
        FindInactive.Find("ConfirmErase").SetActive(false);
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
        if (!usingGravel)
        {
            RemoveItemInventory();
            InteractionText.instance.SetTextTimeout("Usando pederneira... selecione uma tocha para acender");
            usingGravel = true;
        }
        else
        {
            InteractionText.instance.SetTextTimeout("Já está utilizando pederneira, acenda uma tocha primeiro");
        }
    }

    private void UseTorch()
    {
        if (!Inventory[indexSelected].returnActualItem().itemActive)
        {
            if (usingGravel)
            {
                usingGravel = false;
                Inventory[indexSelected].returnActualItem().itemActive = true;
                InteractionText.instance.SetTextTimeout("Tocha acesa");
            }
            else
            {
                InteractionText.instance.SetTextTimeout("Utilize uma pederneira para acender a tocha");
            }
        }
        else
        {
            InteractionText.instance.SetTextTimeout("Tocha já acesa");
        }
    }
    
    private void UseCrowbar()
    {

    }

    #endregion
}