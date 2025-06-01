using UnityEngine;
using UnityEngine.Events; // UnityEvent를 사용하기 위해 필요

public class GenericInteractionExecutor : MonoBehaviour, IGimmickObserver
{
    // Inspector에 노출되어 다양한 함수를 연결할 수 있는 UnityEvent
    [Header("실행할 동작")]
    public UnityEvent onInteraction;

    public void ButtonClick()
    {
        Debug.Log($"[{gameObject.name}] 상호작용 실행! 연결된 동작들을 호출합니다.");
        onInteraction?.Invoke(); // 연결된 모든 함수 실행
    }
    public void OnGimmickEnter()
    {
    }

    public void OnGimmickLeave()
    {
    }
}