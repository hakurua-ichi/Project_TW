using UnityEngine;

public class ElevatorAction : IGimmickAction
{
    private ElevatorAnimator mover;
    private ElevatorState elevatorState;

    public ElevatorAction(GameObject elevatorPlatform)
    {
        mover = elevatorPlatform.GetComponent<ElevatorAnimator>();
        elevatorState = mover.SetState();

        //Debug.Log("[ElevatorAction] 생성자 호출됨");
        //Debug.Log("[ElevatorAction] 초기 상태 IsMoving: " + elevatorState.IsMoving);
    }

    public void Action()
    {
        Debug.Log("[ElevatorAction] Action() 호출됨");
        // 이미 이동 중이라면 실행하지 않음
        if (elevatorState.IsMoving)
        {
            //Debug.Log("[ElevatorAction] 현재 엘리베이터가 이동 중이므로 동작하지 않음");
            return;
        }

        // 이동 시작
        if(!elevatorState.IsMoving)
        {
            elevatorState.IsMoving = true;
            //Debug.Log("[ElevatorAction] IsMoving 설정됨: " + elevatorState.IsMoving);
            mover.StartElevator();
            //Debug.Log("[ElevatorAction] mover.StartElevator() 호출됨");
        }
    }

    public void Exit()
    {
        //Debug.Log("[ElevatorAction] Exit() 호출됨");

        elevatorState.IsMoving = false;
        mover.StopElevator();

        //Debug.Log("[ElevatorAction] 엘리베이터 정지. IsMoving: " + elevatorState.IsMoving);
    }

    public void setup()
    {
        //Debug.Log("[ElevatorAction] setup() 호출됨");
        Debug.Log("elevatorState: " + elevatorState);
        elevatorState.IsMoving = false;
        mover.StopElevator();

        //Debug.Log("[ElevatorAction] 엘리베이터 초기화. IsMoving: " + elevatorState.IsMoving);
    }
}