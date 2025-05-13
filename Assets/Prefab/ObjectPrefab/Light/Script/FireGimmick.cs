using UnityEngine;

public class FireGimmick : MonoBehaviour, IGimmickObserver
{
    [SerializeField] private GimmickSubject TriggerObject;
    [SerializeField] private ParticleSystem Fire;
    private bool lightState = false;
    private GimmickContext context;


    private void Start()
    {
        // 전략 세팅
        context = new GimmickContext();
        context.SetAction(new FireToggleAction(Fire));

        // 옵저버 등록
        var subject = GetComponent<GimmickSubject>();
        if (TriggerObject != null)
        {
            Debug.Log("Fire 옵저버 등록 성공");
            TriggerObject.AddObserverEnter(this); // Light 상태관리 옵저버
        }
        else
        {
            Debug.LogWarning("GimmickSubject가 Fire 오브젝트에 없습니다.");
        }
    }

    public void OnGimmickTriggered()
    {

    }

    public void ButtonClick()
    {
        Debug.Log("Fire 실행");
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
