using UnityEngine;

public class PickUpGimmick : MonoBehaviour, IGimmickObserver
{
    [SerializeField] InventoryItem itemData;

    public void ButtonClick()
    {
        bool ok = SingleSlotInventory.Instance.PickupItem(itemData);
        if (ok)
            Destroy(gameObject);
        else
            Debug.Log("½½·ÔÀÌ °¡µæ Ã¡½À´Ï´Ù!");
    }
    // OnGimmickEnter/Leave Àº ºó ±¸Çö
    public void OnGimmickEnter() { }
    public void OnGimmickLeave() { }
}
