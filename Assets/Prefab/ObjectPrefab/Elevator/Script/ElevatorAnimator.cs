using UnityEngine;

public class ElevatorAnimator : MonoBehaviour
{
    public Transform topPoint;
    public Transform bottomPoint;
    public float speed = 2f;

    private Rigidbody rb;
    private Transform target;
    private bool isMoving = false;

    private ElevatorState elevatorState;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        target = topPoint;
        elevatorState = new ElevatorState();

        Debug.Log("[ElevatorAnimator] Start() 호출됨");
        Debug.Log("[ElevatorAnimator] 초기 목표 지점: " + target.position);
        Debug.Log("[ElevatorAnimator] ElevatorState.IsMoving: " + elevatorState.IsMoving);
    }

    public void StartElevator()
    {
        elevatorState.IsMoving = true;
        Debug.Log("[ElevatorAnimator] StartElevator() 호출됨 - IsMoving: " + elevatorState.IsMoving);
    }

    public void StopElevator()
    {
        elevatorState.IsMoving = false;
        Debug.Log("[ElevatorAnimator] StopElevator() 호출됨 - IsMoving: " + elevatorState.IsMoving);
    }

    public ElevatorState SetState()
    {
        Debug.Log("[ElevatorAnimator] SetState() 호출됨 - ElevatorState 반환");
        return elevatorState;
    }

    void FixedUpdate()
    {
        if (!elevatorState.IsMoving) return;

        // 이동하는 동안 업데이트
        Vector3 newPos = Vector3.MoveTowards(transform.position, target.position, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            elevatorState.IsMoving = false;

            Debug.Log("[ElevatorAnimator] 도착 - 위치: " + transform.position);
            Debug.Log("[ElevatorAnimator] IsMoving = false");

            SwitchTarget();  // 도착했을 때만 목표를 변경
        }
    }

    // 목표를 전환하는 메서드 (상단/하단 이동)
    private void SwitchTarget()
    {
        target = (target == topPoint) ? bottomPoint : topPoint;
        Debug.Log("[ElevatorAnimator] SwitchTarget() 호출됨 - 다음 목표 지점: " + target.position);
    }
}
