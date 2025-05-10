using UnityEngine;

public class RotationInterpolator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float positionSpeed = 10f; // 위치 이동 속도 추가
    
    private Transform cameraTransform;
    private Quaternion targetRotation;
    private Vector3 targetPosition; // 목표 위치 변수 추가
    private bool isRotating = false;
    
    // 회전 완료 이벤트
    // 구독자:
    // - CameraSystemController.OnRotationFinished(float finalAngle)
    //   위치: Assets\Script\Camera\Camera_Movement\Script\CameraSystemController.cs
    public event System.Action<float> RotationFinished;
    
    // 프로퍼티
    public bool IsRotating => isRotating;
    
    private void Awake()
    {
        cameraTransform = transform;
    }
    
    // 회전 시작
    public void StartRotation(Vector3 position, Quaternion rotation, float newAngle)
    {
        // 새 위치와 회전 목표 설정 - 즉시 이동하지 않음
        targetPosition = position; // 위치는 목표값으로 저장
        targetRotation = rotation;
        
        isRotating = true;
    }
    
    // 매 프레임 회전 업데이트
    public void UpdateRotation()
    {
        if (!isRotating) return;
        
        // 위치 보간 추가
        cameraTransform.position = Vector3.Lerp(
            cameraTransform.position,
            targetPosition,
            Time.deltaTime * positionSpeed
        );
        
        // 회전 보간
        cameraTransform.rotation = Quaternion.Lerp(
            cameraTransform.rotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );
        
        // 충분히 가까워지면 완료 (위치와 회전 모두 확인)
        bool rotationComplete = Quaternion.Angle(cameraTransform.rotation, targetRotation) < 0.5f;
        bool positionComplete = Vector3.Distance(cameraTransform.position, targetPosition) < 0.2f;
        
        if (rotationComplete && positionComplete)
        {
            FinalizeRotation();
        }
    }
    
    // 회전 완료 처리
    private void FinalizeRotation()
    {
        // 정확한 90도 단위로 회전 처리
        float roundedY = Mathf.Round(targetRotation.eulerAngles.y / 90f) * 90f;
        cameraTransform.rotation = Quaternion.Euler(0, roundedY, 0);
        
        // 정확한 위치 설정
        cameraTransform.position = targetPosition;
        
        isRotating = false;
        
        // 여기서 카메라가 플레이어를 바라보는 방향이므로 180도를 뺀 값이 실제 카메라의 회전 각도
        // 180도를 더하는 코드는 RotationPositionCalculator.cs에 있음
        float finalAngle = (roundedY - 180f) % 360f;
        if (finalAngle < 0) finalAngle += 360f;
        
        Debug.Log($"[회전 인터폴레이터] 회전 완료: UI각도={roundedY}°, 실제각도={finalAngle}°");
        
        // 회전 완료 이벤트 발생
        // 구독자:
        // - CameraSystemController.OnRotationFinished(float finalAngle)
        //   위치: Assets\Script\Camera\Camera_Movement\Script\CameraSystemController.cs
        RotationFinished?.Invoke(finalAngle);
    }
}