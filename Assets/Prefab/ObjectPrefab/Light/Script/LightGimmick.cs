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
