using UnityEngine;

public class ElevatorPlatformMover : MonoBehaviour
{
    public Transform topPoint;
    public Transform bottomPoint;
    public float speed = 2f;
    public float waitTime = 1.5f;

    private Rigidbody rb;
    private Transform target;
    private bool isMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        target = topPoint;
    }

    void FixedUpdate()
    {
        if (!isMoving) return;

        Vector3 newPos = Vector3.MoveTowards(transform.position, target.position, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            isMoving = false;
            SwitchTarget();  // 도착했을 때만 다음 목표 설정
        }
    }

    public void StartElevator()
    {
        if (!isMoving)
        {
            // 현재 목표를 바꾸지 않고 이동 시작
            isMoving = true;
        }
    }

    private void SwitchTarget()
    {
        target = (target == topPoint) ? bottomPoint : topPoint;
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}