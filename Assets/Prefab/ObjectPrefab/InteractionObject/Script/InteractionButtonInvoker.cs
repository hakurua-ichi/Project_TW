using UnityEngine;
using UnityEngine.UI;

public class InteractionButtonInvoker : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GimmickSubject targetSubject;

    private void Start()
    {
        if (button != null && targetSubject != null)
        {
            button.onClick.AddListener(() =>
            {
                Debug.Log("버튼 클릭됨 → GimmickSubject.Notify()");
                targetSubject.Notify(); // LightGimmick 작동
            });
        }
        else
        {
            Debug.LogWarning("버튼 또는 GimmickSubject가 설정되지 않았습니다.");
        }
    }
}
