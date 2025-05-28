using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class ItemPickup : MonoBehaviour, IGimmickObserver   // 중요!
{
    [SerializeField] private string itemId;   // 예: "Key", "RedGem" …
    public string ItemId => itemId;
    [SerializeField] private Sprite icon;   // 인벤토리에서 보일 아이콘
    public Sprite Icon => icon;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    /* ------------- IGimmickObserver ------------- */
    public void OnGimmickEnter() { /* 필요 시 반짝임 등 */ }
    public void OnGimmickLeave() { /* 필요 시 해제 */     }

    public void ButtonClick()
    {
        // 인벤토리에 넣기 시도
        InventoryManager.Instance.TryPickup(this);
    }
}
