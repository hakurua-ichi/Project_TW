using UnityEngine;

public class ElevatorAction : IGimmickAction
{
    private ElevatorGimmick _elevatorGimmick;
    private Animator animator;
    private Transform elevatorTransform;
    private float elevatorPosition;
    private bool isAtSecondFloor = false;

    public ElevatorAction(GameObject elevatorObject, float floorHeight)
    {
        elevatorTransform = elevatorObject.transform;
        animator = elevatorObject.GetComponent<Animator>();
        _elevatorGimmick = elevatorObject.GetComponent<ElevatorGimmick>();
        elevatorPosition = floorHeight;
        if (animator == null) Debug.LogError("Animator가 문 오브젝트에 없습니다!");
    }

    public void Action()
    {
        float height = elevatorTransform.position.y - elevatorPosition * 5;

        if (height == 0)
        {
            Debug.Log($"엘리베이터 위치: {elevatorPosition}층");
            Debug.Log("엘리베이터 하강 애니메이션 실행");
            animator.SetTrigger("ElevatorDown");
        }
        else
        {
            Debug.Log("엘리베이터 위치: 1층");
            Debug.Log("엘리베이터 상승 애니메이션 실행.");
            animator.SetTrigger("ElevatorUp");
        }    
    }

    public void Execute()
    {
        _elevatorGimmick.ResetisAction();
        Debug.Log("트리거 활성화 완료");
        animator.SetTrigger("ElevatorOut");
    }

    public void setup()
    {

    }
}
