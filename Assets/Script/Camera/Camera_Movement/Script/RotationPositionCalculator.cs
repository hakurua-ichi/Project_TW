using System;
using UnityEngine;

public class RotationPositionCalculator : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float cameraDistance = 50f;
    
    private float currentAngle = 0f;
    private float currentHeight = 10f;
    
    // 위치 계산 결과 이벤트
    public event System.Action<Vector3, Quaternion, float> PositionCalculated;
    
    // 초기화 메서드
    public void Initialize(Transform player, float initialHeight, float initialAngle)
    {
        playerTransform = player;
        currentHeight = initialHeight;
        currentAngle = initialAngle;
    }
    
    // 새 회전 각도로 위치 계산
    public void CalculateNewPosition(float angleChange)
    {
        if (playerTransform == null) return;
        
        if (Math.Abs(currentAngle) > 360f) currentAngle =0;
        // 현재 각도에 회전량 추가
        currentAngle += angleChange;
        
        // 극좌표계로 위치 계산
        float radians = currentAngle * Mathf.Deg2Rad;
        float x = Mathf.Sin(radians) * cameraDistance;
        float z = Mathf.Cos(radians) * cameraDistance;
        
        // 플레이어 기준 카메라 위치
        Vector3 newPosition = playerTransform.position + new Vector3(x, currentHeight, z);
        
        // 목표 회전 계산
        Quaternion targetRotation = Quaternion.Euler(0, currentAngle + 180, 0);
        
        // 계산 결과 이벤트 발생
        PositionCalculated?.Invoke(newPosition, targetRotation, currentAngle);
        
        Debug.Log($"위치 계산: 각도={currentAngle}, 좌표={newPosition}");
    }
    
    // 현재 각도 반환 (외부에서 필요할 때)
    public float GetCurrentAngle() => currentAngle;
    
    public void SetCurrentAngle(float angle)
    {
        currentAngle = angle;
        Debug.Log($"[회전 계산] 현재 각도 업데이트: {currentAngle}°");
    }
}