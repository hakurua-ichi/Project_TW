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

        Collider[] hits = Physics.OverlapBox(transform.position + boxOffset, boxSize * 0.5f);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Rigidbody rb = hit.attachedRigidbody;
                if (rb != null)
                {
                    // �߷¿� ���� �ݹ� ����
                    rb.linearVelocity = Vector3.zero;

                    // �ε巴�� ���� �̵�
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