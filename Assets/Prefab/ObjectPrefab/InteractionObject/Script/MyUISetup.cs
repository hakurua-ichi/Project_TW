using UnityEngine;

public class MyUISetup : MonoBehaviour
{
    [SerializeField] private GimmickSubject trigger;
    [SerializeField] private GameObject buttonUI;

    void Start()
    {
        if (trigger == null || buttonUI == null)
        {
            Debug.LogWarning("트리거나 UI 버튼이 지정되지 않았습니다.");
            return;
        }

        // Enter 시 버튼 표시
        var enterObserver = InteractionObjectController.Attach(this.gameObject, buttonUI, true);
        trigger.AddObserverEnter(enterObserver);

        // Exit 시 버튼 숨김
        var exitObserver = InteractionObjectController.Attach(this.gameObject, buttonUI, false);
        trigger.AddObserverExit(exitObserver);
    }
}