using UnityEngine;

public class CameraRotationController : MonoBehaviour
{
    // 사용하지않는 레거시 스크립트. 추후에 참고할수도 있으니 삭제하지않고 보관함.
    // Camera_Movement.cs로 통합됨.
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float rotationSpeed = 0.1f;
    [SerializeField] private float cameraDistance = 50f;
    [SerializeField] private float followSpeed = 4f; // 추적 속도
    [SerializeField] private float offsetThreshold = 2f; // 추적 임계값
    
    private Transform cameraTransform;
    private Quaternion targetRotation;
    private bool isRotating = false;
    private float currentDistance;
    private float currentHeight;
    private float currentAngle;
    
    // 축별 추적 상태
    private bool onXTrace = true; // 기본적으로 X축 추적 활성화
    private bool onZTrace = false;
    
    // 다른 카메라 스크립트 참조
    private CameraFollow playerTraceScript;
    
    void Start()
    {
        cameraTransform = transform;
        // cameraTransform = Camera.main.transform; // 메인 카메라로 설정
        targetRotation = cameraTransform.rotation;
        
        // Player_trace 스크립트 참조 가져오기
        playerTraceScript = GetComponent<CameraFollow>();
        if (playerTraceScript != null)
        {
            // 다른 추적 스크립트 비활성화
            playerTraceScript.enabled = false;
        }
        
        // 초기 카메라 설정
        InitializeCameraValues();
        
        // 초기 회전 각도에 따른 추적 모드 설정
        UpdateTrackingMode();
    }
    
    private void InitializeCameraValues()
    {
        // 기존 초기화 코드...
        Vector3 offset = cameraTransform.position - playerTransform.position;
        currentHeight = offset.y;
        currentDistance = new Vector3(offset.x, 0, offset.z).magnitude;
        currentAngle = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
        
        Debug.Log($"카메라 초기화: 거리={currentDistance}, 높이={currentHeight}, 각도={currentAngle}");
    }
    
    void LateUpdate() // Update 대신 LateUpdate 사용하여 다른 스크립트와의 충돌 방지
    {
        // 회전 중이 아니면 입력 감지 및 플레이어 추적
        if (!isRotating)
        {
            // 오른쪽 회전
            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateAroundPlayer(90f);
            }
            // 왼쪽 회전
            else if (Input.GetKeyDown(KeyCode.T))
            {
                RotateAroundPlayer(-90f);
            }
            
            // 카메라 위치 추적 처리
            TrackPlayer();
        }
        
        // 회전 중이면 보간 처리
        if (isRotating)
        {
            // 속도가 너무 빠르다. 그렇다고 deltaTime을 뺴면 그냥 순간이동 해버린다.
            cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            
            // 목표 회전에 충분히 근접하면 보정
            if (Quaternion.Angle(cameraTransform.rotation, targetRotation) < 2f)
            {
                // 현재 회전값의 오일러 각도 가져오기
                Vector3 currentEuler = targetRotation.eulerAngles;
                
                // Y축 회전값을 90도 단위로 반올림
                float roundedY = Mathf.Round(currentEuler.y / 90f) * 90f;
                
                float xAngle = 0f; // 카메라가 수평을 유지하려면 0으로 설정
                
                // 반올림된 회전값으로 최종 회전 적용
                cameraTransform.rotation = Quaternion.Euler(xAngle, roundedY, 0f);
                targetRotation = cameraTransform.rotation; // 목표 회전값도 업데이트
                
                isRotating = false;
                
                // 회전이 완료된 후 추적 모드 업데이트
                UpdateTrackingMode();
            }
        }
    }
    
    // 회전 각도에 따라 추적 모드 설정
    private void UpdateTrackingMode()
    {
        // Y축 회전값 가져오기 (0~360으로 정규화)
        float yRotation = Mathf.RoundToInt(cameraTransform.eulerAngles.y) % 360;
        
        // 각도에 따라 추적 모드 설정
        if (yRotation == 0 || yRotation == 180)
        {
            // 0도 또는 180도: X축만 추적
            onXTrace = true;
            onZTrace = false;
            Debug.Log($"추적 모드: X축만 (회전 각도: {yRotation}°)");
        }
        else if (yRotation == 90 || yRotation == 270)
        {
            // 90도 또는 270도: Z축만 추적
            onXTrace = false;
            onZTrace = true;
            Debug.Log($"추적 모드: Z축만 (회전 각도: {yRotation}°)");
        }
    }
    
    // 플레이어 추적 로직
    private void TrackPlayer()
    {
        if (playerTransform == null) return;
        
        Vector3 currentPos = transform.position;
        Vector3 playerPos = playerTransform.position;
        Vector3 newPos = currentPos;
        
        // X축 추적 (onXTrace가 true일 때만)
        if (onXTrace && Mathf.Abs(playerPos.x - currentPos.x) > offsetThreshold)
        {
            newPos.x = Mathf.Lerp(currentPos.x, playerPos.x, Time.deltaTime * followSpeed);
        }
        
        // Z축 추적 (onZTrace가 true일 때만)
        if (onZTrace && Mathf.Abs(playerPos.z - currentPos.z) > offsetThreshold)
        {
            newPos.z = Mathf.Lerp(currentPos.z, playerPos.z, Time.deltaTime * followSpeed);
        }
        
        // 높이는 항상 유지
        newPos.y = currentPos.y;
        
        // 새 위치 적용
        transform.position = newPos;
    }
    
    private void RotateAroundPlayer(float angle)
    {
        // 현재 각도에 회전량 추가
        currentAngle += angle;
        
        // 항상 고정된 거리 사용
        float radians = currentAngle * Mathf.Deg2Rad;
        float x = Mathf.Sin(radians) * cameraDistance;
        float z = Mathf.Cos(radians) * cameraDistance;
        
        // 플레이어 위치 + 회전된 오프셋 = 새 카메라 위치
        Vector3 newPosition = playerTransform.position + new Vector3(x, currentHeight, z);
        cameraTransform.position = newPosition;
        
        // 목표 회전값 업데이트 - 정확한 각도 사용
        targetRotation = Quaternion.Euler(0, currentAngle + 180, 0);
        
        // 회전 시작
        isRotating = true;
        
        Debug.Log($"회전: 각도={currentAngle}, 새 위치={newPosition}");
    }
}
