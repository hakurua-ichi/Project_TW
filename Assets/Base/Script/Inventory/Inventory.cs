using UnityEngine;

public class Inventory : MonoBehaviour
{
    public bool hasItem = false;
    public string itemName = "";
    
    public void GetItem(string itemName)
    {
        if (hasItem) { return; }
        else
        {
            Debug.Log("아이템 줍기");
            hasItem = true;
            this.itemName = itemName;
        }
    }
}
