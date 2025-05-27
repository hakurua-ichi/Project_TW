using TMPro;
using UnityEngine;

public class PickUpAction : IGimmickAction
{
    private Inventory inventory;
    private string itemName;

    public PickUpAction(Inventory inventory, string itemName)
    {
        this.inventory = inventory;
        this.itemName = itemName;
    }

    public void Action()
    {
        Debug.Log($"PickUpAction: æ∆¿Ã≈€ '{itemName}'¿ª(∏¶) »πµÊ«’¥œ¥Ÿ.");
        inventory.GetItem(itemName);
    }

    public void Exit() { }

    public void Setup() { }
}
