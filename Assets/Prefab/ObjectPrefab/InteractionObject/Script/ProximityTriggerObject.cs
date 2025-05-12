using UnityEngine;

[RequireComponent(typeof(GimmickSubject))]
public class ProximityTriggerObject : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 3f;
    [SerializeField] private Transform playerTransform;

    private GimmickSubject subject;
    private bool isPlayerInRange = false;

    private void Awake()
    {
        subject = GetComponent<GimmickSubject>();

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= detectionRadius && !isPlayerInRange)
        {
            isPlayerInRange = true;
            subject.Notify(); // Enter 알림
        }
        else if (distance > detectionRadius && isPlayerInRange)
        {
            isPlayerInRange = false;
            subject.NotifyExit(); // Exit 알림
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
