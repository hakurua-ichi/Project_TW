using UnityEngine;
using UnityEngine.UI;

public class GimmickButtonTrigger : MonoBehaviour
{
    [Header("UI 버튼 설정")]
    [SerializeField] private Button button;

    [Header("Gimmick Subject가 있는 오브젝트")]
    [SerializeField] private GameObject targetObject; // 여기에 GimmickSubject가 있어야 함

    private void Start()
    {
        if (button == null || targetObject == null)
        {
            Debug.LogWarning("버튼 또는 타겟 오브젝트가 설정되지 않았습니다.");
            return;
        }

        GimmickSubject subject = targetObject.GetComponent<GimmickSubject>();
        if (subject == null)
        {
            Debug.LogError("Target Object에 GimmickSubject가 없습니다.");
            return;
        }

        // 현재 GimmickObserver를 찾아서 등록
        IGimmickObserver[] observers = targetObject.GetComponents<IGimmickObserver>();
        foreach (var observer in observers)
        {
            subject.AddObserverEnter(observer); // 버튼 클릭용으로 등록
        }

        // 버튼 클릭 시 Notify 호출
        button.onClick.AddListener(() =>
        {
            Debug.Log("버튼 클릭됨 → Notify");
            subject.Notify();
        });
    }
}