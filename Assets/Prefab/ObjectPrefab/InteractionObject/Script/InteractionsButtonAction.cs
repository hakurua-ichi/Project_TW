using TMPro;
using UnityEngine;

public class InteractionsButtonAction : MonoBehaviour
{
    public static InteractionsButtonAction Instance { get; private set; }

    [SerializeField] private GameObject uiRoot;   // 실제 버튼 오브젝트
    private ProximityTriggerObject currentTrigger;
    private TextMeshProUGUI buttonText;

    void Awake()
    {
        buttonText = uiRoot.GetComponentInChildren<TextMeshProUGUI>();
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        uiRoot.SetActive(false);

    }

    // 트리거가 ‘나를 선택해 달라’고 호출
    public void RequestSelection(ProximityTriggerObject trigger, GameObject objectName)
    {
        // ① 더 가까우면 교체
        if (currentTrigger == null ||
            trigger.DistanceToPlayer < currentTrigger.DistanceToPlayer)
        {
            currentTrigger = trigger;
            buttonText.text = objectName.name; // 버튼 텍스트 변경
            uiRoot.SetActive(true);
        }
    }

    // 트리거가 범위를 벗어났다고 알림
    public void NotifyExit(ProximityTriggerObject trigger)
    {
        if (trigger == currentTrigger)
        {
            currentTrigger = ProximityTriggerObject.GetClosestInRange();
            uiRoot.SetActive(currentTrigger != null);
        }
    }

    // 버튼 클릭 시 호출
    public void OnButtonClicked()
    {
        Debug.Log("Button Clicked");
        currentTrigger.InvokeButton();   // Subject.NotifyExit() 내부에서 실행
    }
}


//using UnityEngine;

//public class InteractionsButtonAction : MonoBehaviour
//{
//    private ProximityTriggerObject proximityTriggerObject;

//    public void SetCurrentTriggerObject(ProximityTriggerObject trigger)
//        => proximityTriggerObject = trigger;

//    public void ButtonClicked()
//    {
//        if (proximityTriggerObject == null)
//        {
//            Debug.LogWarning("ProximityTriggerObject가 설정되지 않았습니다.");
//            return;
//        }

//        var subject = proximityTriggerObject.GetSubject();
//        var observer = proximityTriggerObject.GetObserver();

//        if (subject == null || observer == null)
//        {
//            Debug.LogWarning("Subject/Observer를 찾지 못했습니다.");
//            return;
//        }

//        // 안전 장치: 혹시 빠져 있으면 등록
//        subject.AddExitObserver(observer);              // 보강

//        // 실제 토글 동작 > ButtonClick() 브로드캐스트
//        subject.NotifyExit();                           // 변경 (매 클릭마다)
//    }
//}

#region 구형코드
/*
구형 코드
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
*/
#endregion