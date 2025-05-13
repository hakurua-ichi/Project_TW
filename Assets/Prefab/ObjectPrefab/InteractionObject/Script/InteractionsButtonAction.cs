using UnityEngine;
using System.Linq;


public class InteractionsButtonAction : MonoBehaviour
{
    private ProximityTriggerObject proximityTriggerObject; // 현재 근처의 기믹 오브젝트
    private bool buttonState = false;

    public void SetCurrentTriggerObject(ProximityTriggerObject trigger)
    {
        proximityTriggerObject = trigger;
    }

    public void ButtonClicked()
    {
        if (proximityTriggerObject == null)
        {
            Debug.LogWarning("ProximityTriggerObject가 설정되지 않았습니다.");
            return;
        }

        string actionName = proximityTriggerObject.GetActionObjectName();

        var allGimmicks = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IGimmickObserver>();

        foreach (var gimmick in allGimmicks)
        {
            if (gimmick is MonoBehaviour mb && mb.gameObject.name == actionName)
            {
                GimmickSubject subject = proximityTriggerObject.GetComponent<GimmickSubject>();

                if (!buttonState)
                    subject.Notify(gimmick);
                else
                    subject.NotifyExit(gimmick);

                buttonState = !buttonState;
                return;
            }
        }

        Debug.LogWarning($"이름이 {actionName}인 IGimmickObserver를 찾지 못했습니다.");
    }
}