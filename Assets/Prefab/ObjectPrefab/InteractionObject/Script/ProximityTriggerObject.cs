using UnityEngine;

[RequireComponent(typeof(GimmickSubject))]
public class ProximityTriggerObject : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 3f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject button_UI;
    private bool isPlayerInRange = false;

    private void Awake()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }

    private void Start()
    {
        button_UI.SetActive(false);
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= detectionRadius && !isPlayerInRange)
        {
            isPlayerInRange = true;
            button_UI.SetActive(isPlayerInRange);
        }
        else if (distance > detectionRadius && isPlayerInRange)
        {
            isPlayerInRange = false;
            button_UI.SetActive(isPlayerInRange);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
