using UnityEngine;

public class Inventory : MonoBehaviour
{
    public bool hasItem = false;
    private string itemName = "";

    public string ItemName()
    {
        return itemName;
    }

    public void GetItem(string itemName)
    {
        if (hasItem) { return; }
        else
        {
            Debug.Log("Inventory: 아이템 줍기");
            hasItem = true;
            this.itemName = itemName;
        }
    }

    public void DropItem(string ItemName)
    {
        if(!hasItem) { return; }
        else
        {
            Debug.Log("Inventory: 아이템 버리기");
            hasItem = false;
            itemName = "";
        }
    }
}
