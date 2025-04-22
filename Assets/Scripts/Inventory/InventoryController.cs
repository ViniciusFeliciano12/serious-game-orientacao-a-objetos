using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance { get; private set; }

    private RectTransform craftArea;
    public ItemInventoryController[] Inventory;
    private void Awake()
    {
        if(Instance!= null && Instance != this)
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

        var inventoryDatabase = GameController.Instance.GetInventory();

        for (int i = 0; i < inventoryDatabase.Count; i++)
        {
            Inventory[i].InstantiateItem(inventoryDatabase[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        SelectItemInventory();
    }

    public void UpdateInventory(){
        var inventoryDatabase = GameController.Instance.GetInventory();

        for(int i = 0; i < inventoryDatabase.Count; i++){
            Inventory[i].InstantiateItem(inventoryDatabase[i]);
        }
    }

    private void SelectItemInventory(){
        if(Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)){
            SelectItemAt(9);
        }
        if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)){
            SelectItemAt(0);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)){
            SelectItemAt(1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)){
            SelectItemAt(2);
        }
        if(Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)){
            SelectItemAt(3);
        }
        if(Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)){
            SelectItemAt(4);
        }
        if(Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)){
            SelectItemAt(5);
        }
        if(Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)){
            SelectItemAt(6);
        }
        if(Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)){
            SelectItemAt(7);
        }
        if(Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9)){
            SelectItemAt(8);
        }
    }

    public void SelectItemAt(int index){
        for(int i=0; i<Inventory.Count(); i++){
            if(i == index){
                Inventory[i].ItemSelected(true);
            }else{
                Inventory[i].ItemSelected(false);
            }
        }
    }
}
