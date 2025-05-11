using UnityEngine;

public class PlayerElevatorDetector : MonoBehaviour
{
    public Vector3 boxSize = new Vector3(5f, 1f, 5f);
    public Vector3 boxOffset = new Vector3(0f, 1f, 0f);

    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        Vector3 delta = transform.position - lastPosition;

        // 엘리베이터가 움직이는지 확인
        bool isElevatorMoving = delta.sqrMagnitude > 0.0001f;

        if (!isElevatorMoving)
        {
            lastPosition = transform.position;
            return; // 움직이지 않으면 처리 안 함
        }

        Collider[] hits = Physics.OverlapBox(transform.position + boxOffset, boxSize * 0.5f);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Rigidbody rb = hit.attachedRigidbody;
                if (rb != null)
                {
                    // 움직이는 동안만 델타 적용
                    rb.MovePosition(rb.position + delta);
                }
            }
        }

        lastPosition = transform.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + boxOffset, boxSize);
    }
}