using UnityEngine;

public class LightGimmick : MonoBehaviour, IGimmickObserver
{
    private GimmickContext context;

    [SerializeField] private Light targetLight;
    [SerializeField] private GimmickSubject subjectOn; // Subject 연결 필드
    [SerializeField] private GimmickSubject subjectOff; // Subject 연결 필드

    private void Start()
    {
        targetLight.enabled = false;  // 시작할 때 꺼두기

        // 전략 세팅
        context = new GimmickContext();
        context.SetAction(new LightToggleAction(targetLight));

        // 옵저버 등록
        if (subjectOn != null)
        {
            Debug.Log("불켜기 옵저버 등록 성공");
            subjectOn.AddObserver(this); // 여기서 옵저버 등록
        }
        else
        {
            Debug.LogWarning("불켜기 옵저버 등록 실패");
        }

        if (subjectOff != null)
        {
            Debug.Log("불끄기 옵저버 등록 성공");
            subjectOff.AddObserver(new ExitObserver(context)); // 여기서 옵저버 등록
        }
        else
        {
            Debug.LogWarning("불끄기 옵저버 등록 실패");
        }
    }

    public void OnGimmickTriggered()
    {
        Debug.Log("Light기믹 실행");
        context.ExecuteAction();
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
    }
}
