using UnityEngine;

public class RotationPositionCalculator : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    
    [Header("카메라 위치 설정")]
    [Tooltip("카메라와 플레이어 사이의 거리")]
    [SerializeField] private float cameraDistance = 5f;
    
    [Tooltip("카메라의 높이 (플레이어 기준)")]
    [SerializeField] private float cameraHeight = 2f;
    
    private float currentAngle = 0f;
    
    // 위치 계산 결과 이벤트
    // 구독자:
    // - RotationInterpolator.StartRotation(Vector3 position, Quaternion rotation, float newAngle)
    //   위치: Assets\Script\Camera\Camera_Movement\Script\RotationInterpolator.cs
    public event System.Action<Vector3, Quaternion, float> PositionCalculated;
    
    // 초기화 메서드
    public void Initialize(Transform player, float initialHeight, float initialAngle)
    {
        playerTransform = player;
        currentAngle = initialAngle;
        
        // 높이 값이 외부에서 지정되었을 경우 적용
        if (initialHeight > 0)
            cameraHeight = initialHeight;
    }
    
    // 새 회전 각도로 위치 계산
    public void CalculateNewPosition(float angleChange)
    {
        if (playerTransform == null) return;
        
        // 현재 카메라 위치와 플레이어 위치 가져오기
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 playerPosition = playerTransform.position;
        
        // 플레이어에서 카메라로의 방향 벡터 계산
        Vector3 directionToCamera = cameraPosition - playerPosition;
        
        // 현재 높이 저장 (외부에서 지정된 높이 우선 사용)
        float height = cameraHeight;
        
        // 현재 거리 저장 (외부에서 지정된 거리 우선 사용)
        float distance = cameraDistance;
        
        // 현재 각도 계산 (arctan2를 사용하여 현재 방향 각도 계산)
        float currentDegrees = Mathf.Atan2(directionToCamera.x, directionToCamera.z) * Mathf.Rad2Deg;
        
        // 새 각도 계산 (현재 각도에 회전량 더하기)
        float newDegrees = currentDegrees + angleChange;
        
        // 90도 단위로 반올림하여 정확한 회전 보장
        newDegrees = Mathf.Round(newDegrees / 90f) * 90f;
        
        // 새로운 방향 벡터 계산
        float newRadians = newDegrees * Mathf.Deg2Rad;
        float newX = Mathf.Sin(newRadians) * distance;
        float newZ = Mathf.Cos(newRadians) * distance;
        
        // 새로운 카메라 위치 계산
        Vector3 newPosition = playerPosition + new Vector3(newX, height, newZ);
        
        // 플레이어를 바라보는 회전 계산
        Quaternion targetRotation = Quaternion.Euler(0, newDegrees + 180, 0);
        
        // 현재 각도 업데이트
        currentAngle = newDegrees;
        
        // 계산 결과 이벤트 발생
        PositionCalculated?.Invoke(newPosition, targetRotation, currentAngle);
    }
    
    // 현재 각도 반환 (외부에서 필요할 때)
    public float GetCurrentAngle() => currentAngle;
    
    // 거리와 높이 setter/getter
    public float GetCameraDistance() => cameraDistance;
    public float GetCameraHeight() => cameraHeight;
    
    public void SetCameraDistance(float distance)
    {
        cameraDistance = distance;
    }
    
    public void SetCameraHeight(float height)
    {
        cameraHeight = height;
    }
    
    public void SetCurrentAngle(float angle)
    {
        currentAngle = angle;
    }
}