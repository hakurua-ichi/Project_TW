using UnityEngine;

public class RotateMap : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private KeyCode leftRotateKey = KeyCode.Q;
    [SerializeField] private KeyCode rightRotateKey = KeyCode.E;
    [SerializeField] private Transform pivotPoint;

    [Header("디버그 정보")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private int childCount = 0;

    private bool isRotating = false;
    private float targetRotation = 0f;
    private float currentRotation = 0f;

    void Start()
    {
        if (pivotPoint == null)
        {
            pivotPoint = transform;
            if (showDebugInfo) Debug.Log("[회전 맵] 피봇 포인트가 할당되지 않아 자신으로 설정됨");
        }

        // 초기 회전값을 현재 오브젝트의 Y축 회전으로 설정
        currentRotation = pivotPoint.eulerAngles.y;
        targetRotation = currentRotation;

        childCount = transform.childCount;
        if (showDebugInfo)
        {
            Debug.Log("[회전 맵] 시작 - 자식 오브젝트 수: " + childCount);
            if (childCount == 0 && transform.parent != pivotPoint)
            {
                Debug.LogWarning("[회전 맵] 경고: 자식 오브젝트가 없습니다. 회전할 블록이 없습니다!");
            }
        }
    }

    void Update()
    {
        // 회전 처리 (기존 로직 유지)
        if (isRotating)
        {
            float step = rotationSpeed * Time.deltaTime;
            // 기존 currentRotation과 targetRotation이 Y축 회전만을 다룬다고 가정하고 MoveTowardsAngle 사용
            currentRotation = Mathf.MoveTowardsAngle(currentRotation, targetRotation, step);

            // 회전 적용 (pivotPoint 기준)
            pivotPoint.rotation = Quaternion.Euler(0, currentRotation, 0);

            // 회전 완료 확인
            if (Mathf.Approximately(currentRotation, targetRotation))
            {
                currentRotation = targetRotation; // 정확한 값으로 정렬
                isRotating = false;
                if (showDebugInfo) Debug.Log("[회전 맵] 회전 완료. 최종 각도: " + currentRotation);
            }
        }
    }

    /// <summary>
    /// 맵을 왼쪽으로 90도 회전시킵니다.
    /// GenericInteractionExecutor의 UnityEvent에 연결하여 사용합니다.
    /// </summary>
    public void TriggerLeftRotation()
    {
        if (isRotating)
        {
            if (showDebugInfo) Debug.Log("[회전 맵] 이미 회전 중입니다. 왼쪽 회전 요청 무시.");
            return;
        }
        if (showDebugInfo) Debug.Log("[회전 맵] TriggerLeftRotation() 호출됨.");
        targetRotation = currentRotation - 90f;
        isRotating = true;
    }

    /// <summary>
    /// 맵을 오른쪽으로 90도 회전시킵니다.
    /// GenericInteractionExecutor의 UnityEvent에 연결하여 사용합니다.
    /// </summary>
    public void TriggerRightRotation()
    {
        if (isRotating)
        {
            if (showDebugInfo) Debug.Log("[회전 맵] 이미 회전 중입니다. 오른쪽 회전 요청 무시.");
            return;
        }
        if (showDebugInfo) Debug.Log("[회전 맵] TriggerRightRotation() 호출됨.");
        targetRotation = currentRotation + 90f;
        isRotating = true;
    }

    // Getters (기존 유지)
    public float GettargetRotation() => targetRotation;
    public Transform GetPivotPoint() => pivotPoint;
    public float GetrotationSpeed() => rotationSpeed;
    public bool IsRotating() => isRotating;
}