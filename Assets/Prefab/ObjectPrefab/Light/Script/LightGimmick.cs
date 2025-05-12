using UnityEngine;

public class LightGimmick : MonoBehaviour, IGimmickObserver
{
    [SerializeField] private GimmickSubject TriggerObject;
    [SerializeField] private Light targetLight;

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
            TriggerObject.AddObserverEnter(this); // 불 켜기
            TriggerObject.AddObserverExit(new ExitObserver(context)); // 불 끄기
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
        context.StartAction();
    }

    // 내부 클래스: Light 끄기 전용 옵저버
    private class ExitObserver : IGimmickObserver
    {
        private GimmickContext context;

        public ExitObserver(GimmickContext ctx)
        {
            context = ctx;
        }

        public void OnGimmickTriggered()
        {
            context.CancelAction(); // 불 끄기
        }

        public void ButtonClick()
        {

        }
    }
}
