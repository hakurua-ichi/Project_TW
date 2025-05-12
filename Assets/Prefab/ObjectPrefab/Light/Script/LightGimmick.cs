using UnityEngine;

public class LightGimmick : MonoBehaviour, IGimmickObserver
{
    [SerializeField] private GimmickSubject TriggerObject;
    [SerializeField] private Light targetLight;
    private bool lightState = false;
    private GimmickContext context;


    private void Start()
    {
        // 전략 세팅
        context = new GimmickContext();
        context.SetAction(new LightToggleAction(targetLight));

        // 옵저버 등록
        var subject = GetComponent<GimmickSubject>();
        if (TriggerObject != null)
        {
            Debug.Log("Light 옵저버 등록 성공");
            TriggerObject.AddObserverEnter(this); // Light 상태관리 옵저버
        }
        else
        {
            Debug.LogWarning("GimmickSubject가 Light 오브젝트에 없습니다.");
        }
    }

    public void OnGimmickTriggered()
    {

    }

    public void ButtonClick()
    {
        Debug.Log("Light기믹 실행");
        if (!lightState)
        {
            context.StartAction();
            lightState = true;
        }
        else
        {
            context.CancelAction();
            lightState = false;
        }
    }
}
