using UnityEngine;
using UnityEngine.InputSystem;

public class RotateMap : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private string leftKeyBinding = "<Keyboard>/q"; // 문자열 기반 키 바인딩
    [SerializeField] private string rightKeyBinding = "<Keyboard>/e"; // 문자열 기반 키 바인딩
    [SerializeField] private Transform pivotPoint;
    
    [Header("디버그 정보")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private int childCount = 0;
    
    private bool isRotating = false;
    private float targetRotation = 0f;
    private float currentRotation = 0f;
    
    // Input System 액션 생성
    private InputAction leftRotateAction;
    private InputAction rightRotateAction;
    
    void Awake()
    {
        // 입력 액션 설정
        leftRotateAction = new InputAction("RotateLeft", binding: leftKeyBinding);
        rightRotateAction = new InputAction("RotateRight", binding: rightKeyBinding);
        
        // 액션 활성화
        leftRotateAction.Enable();
        rightRotateAction.Enable();
    }
    
    void OnDestroy()
    {
        // 액션 정리
        leftRotateAction.Dispose();
        rightRotateAction.Dispose();
    }
    
    void Start()
    {
        if (pivotPoint == null)
        {
            pivotPoint = transform;
            Debug.Log("[회전 맵] 피봇 포인트가 할당되지 않아 자신으로 설정됨");
        }
        
        // 자식 오브젝트 개수 확인
        childCount = transform.childCount;
        if (showDebugInfo)
        {
            Debug.Log("[회전 맵] 시작 - 자식 오브젝트 수: " + childCount);
            
            if (childCount == 0)
            {
                Debug.LogWarning("[회전 맵] 경고: 자식 오브젝트가 없습니다. 회전할 블록이 없습니다!");
            }
            
            for (int i = 0; i < childCount; i++)
            {
                //Debug.Log($"[회전 맵] 자식 {i}: {transform.GetChild(i).name}");
            }
        }
    }
    
    void Update()
    {
        // 키 입력 확인
        bool leftKeyDown = leftRotateAction.triggered;
        bool rightKeyDown = rightRotateAction.triggered;
        
        if (showDebugInfo && (leftKeyDown || rightKeyDown))
        {
            Debug.Log("[회전 맵] 키 입력 감지: " + (leftKeyDown ? "왼쪽 회전" : "오른쪽 회전"));
        }
        
        // 왼쪽 회전
        if (leftKeyDown && !isRotating)
        {
            targetRotation = currentRotation - 90f;
            isRotating = true;
        }
        // 오른쪽 회전
        else if (rightKeyDown && !isRotating)
        {
            targetRotation = currentRotation + 90f;
            isRotating = true;
        }
        
        // 회전 처리
        if (isRotating)
        {
            float step = rotationSpeed * Time.deltaTime;
            currentRotation = Mathf.MoveTowards(currentRotation, targetRotation, step);
            
            // 회전 적용
            transform.rotation = Quaternion.Euler(0, currentRotation, 0);
            
            // 회전 완료 확인
            if (Mathf.Approximately(currentRotation, targetRotation))
            {
                isRotating = false;
            }
        }
    }
    // Getters
    public float GettargetRotation()
    {
        return targetRotation;
    }
    public Transform GetPivotPoint()
    {
        return pivotPoint;
    }
    public float GetrotationSpeed()
    {
        return rotationSpeed;
    }
    public bool IsRotating()
    {
        return isRotating;
    }
}
