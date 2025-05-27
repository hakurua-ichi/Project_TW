using TMPro;
using UnityEngine;

public class PickUpGimmick : MonoBehaviour, IGimmickObserver
{
    private GimmickSubject triggerObject;
    private GimmickContext context;
    private Inventory inventory;
    private InventoryUI inventoryUI;

    void Start()
    {
        inventory = GameObject.Find("InteractionButtonManager").GetComponent <Inventory>();
        inventoryUI = GameObject.Find("ItemUsingButton").GetComponent<InventoryUI>();

        context = new GimmickContext();
        context.SetAction(new PickUpAction(inventory, gameObject.name));
    }

    public void ButtonClick()
    {
        Debug.Log("버튼 클릭됨");
        if (inventory.hasItem)
        {
            Debug.Log("인벤토리 가득 참");
            return;
        }
        else
        {
            Debug.Log("PickUpGimmick: 아이템 줍기");
            context.StartAction();
            inventoryUI.SetItemName(inventory);
            Destroy(gameObject);
            Debug.Log("PickUpGimmick: 아이템 파괴");
        }
    }
    // OnGimmickEnter/Leave 은 빈 구현
    public void OnGimmickEnter() { }
    public void OnGimmickLeave() { }
}

//#region GPT산
//using UnityEngine;

//public class PickUpGimmick : MonoBehaviour, IGimmickObserver
//{
//    [SerializeField] InventoryItem itemData;

//    public void ButtonClick()
//    {
//        bool ok = SingleSlotInventory.Instance.PickupItem(itemData);
//        if (ok)
//            Destroy(gameObject);
//        else
//            Debug.Log("슬롯이 가득 찼습니다!");
//    }
//    // OnGimmickEnter/Leave 은 빈 구현
//    public void OnGimmickEnter() { }
//    public void OnGimmickLeave() { }
//}
//#endregion