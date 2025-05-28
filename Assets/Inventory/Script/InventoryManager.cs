using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private Image slotImage;     // 아이콘을 보여줄 Image
    [SerializeField] private Button useButton;    // 버리기/사용 버튼

    [Header("참조")]
    [SerializeField] private Transform player;    // 플레이어 Transform

    [Header("아이템 버리기 설정")]
    [SerializeField] private float dropForwardDistance = 1.0f;
    [SerializeField] private float dropAboveGround = 0.3f;   // 지면 위 30 cm

    private ItemPickup currentItem;               // 보유 중인 아이템(1칸)

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        ClearSlot();                  // 시작 상태 비우기
        useButton.onClick.AddListener(OnUseButton);
    }

    /* ---------- 아이템 줍기 ---------- */
    public bool TryPickup(ItemPickup item)
    {
        if (currentItem != null) return false;    // 이미 차 있음

        currentItem = item;
        slotImage.sprite = item.Icon;
        slotImage.enabled = true;
        useButton.interactable = true;

        item.gameObject.SetActive(false);         // 월드에서 숨김
        return true;
    }

    /* ---------- 버리기/사용 ---------- */
    private void OnUseButton()
    {
        if (currentItem == null) return;

        // 플레이어 앞쪽 기준점
        Vector3 basePos = player.position + player.forward * dropForwardDistance;

        // 위에서 아래로 레이캐스트로 ‘지면 높이’ 찾기
        Vector3 rayStart = basePos + Vector3.up * 2f;          // 머리 위에서 쏘기
        RaycastHit hit;
        float groundY = basePos.y;                             // 기본 높이(못 찾으면)

        if (Physics.Raycast(rayStart, Vector3.down, out hit, 4f, ~0,
                             QueryTriggerInteraction.Ignore))
        {
            groundY = hit.point.y;
        }

        // 최종 드롭 위치 = 지면 + offset
        Vector3 dropPos = new Vector3(basePos.x, groundY + dropAboveGround, basePos.z);

        // 아이템 활성화 & 위치 지정
        currentItem.transform.position = dropPos;
        currentItem.transform.rotation = Quaternion.identity;  // 필요 시
        currentItem.gameObject.SetActive(true);

        // 중간에 Rigidbody가 잠겨 있었으면 풀어 주기
        Rigidbody rb = currentItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        ClearSlot();
    }

    private void ClearSlot()
    {
        currentItem = null;
        slotImage.sprite = null;
        slotImage.enabled = false;
        useButton.interactable = false;
    }

    public void RemoveCurrentItem() => ClearSlot();

    public bool HasItem(string id)
    {
        return currentItem != null && currentItem.ItemId == id;
    }
}
