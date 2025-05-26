using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 1)]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public UnityEvent OnUse;     // 인스펙터에서 사용할 함수 등록

    // 예: 열쇠를 사용하면 해당 문을 여는 콜백을 붙여두고
    // OnUse.Invoke() 하면 연결된 함수가 실행됩니다.
}