using UnityEngine;
using System;

public class SingleSlotInventory : MonoBehaviour
{
    public static SingleSlotInventory Instance { get; private set; }

    // 현재 슬롯에 든 아이템 (없으면 null)
    public InventoryItem CurrentItem { get; private set; }

    // 슬롯 변경 시 UI 갱신을 위해 외부에서 구독
    public event Action<InventoryItem> OnItemChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>
    /// 아이템을 줍기 시도합니다.
    /// 슬롯 비어 있으면 담고 true, 이미 차 있으면 false.
    /// </summary>
    public bool PickupItem(InventoryItem item)
    {
        if (CurrentItem != null)
            return false;

        CurrentItem = item;
        OnItemChanged?.Invoke(CurrentItem);
        return true;
    }

    /// <summary>
    /// 슬롯에 든 아이템을 사용(Consume)합니다.
    /// 사용 로직은 각 아이템마다 달리 구현하세요.
    /// </summary>
    public void UseItem()
    {
        if (CurrentItem == null) return;

        // 예: 포션이면 체력 회복, 열쇠면 문 열기 등
        CurrentItem.OnUse?.Invoke();

        // 사용 후 슬롯 비움
        CurrentItem = null;
        OnItemChanged?.Invoke(null);
    }

    /// <summary>
    /// 아이템 버리기(취소)용
    /// </summary>
    public void DropItem()
    {
        if (CurrentItem == null) return;
        // 필요 시 world에 다시 스폰하는 로직 추가 가능
        CurrentItem = null;
        OnItemChanged?.Invoke(null);
    }
}