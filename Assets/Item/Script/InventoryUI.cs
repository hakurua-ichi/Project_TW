using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Image     slotIcon;
    [SerializeField] private TextMeshProUGUI slotText;
    [SerializeField] private Button    useButton;

    void Start()
    {
        SingleSlotInventory.Instance.OnItemChanged += Refresh;
        useButton.onClick.AddListener(() => SingleSlotInventory.Instance.UseItem());
        Refresh(null);
    }

    void Refresh(InventoryItem item)
    {
        bool has = item != null;
        slotIcon.sprite    = has ? item.icon : null;
        slotIcon.enabled   = has;
        slotText.text      = has ? item.itemName : "Empty";
        useButton.interactable = has;
    }
}