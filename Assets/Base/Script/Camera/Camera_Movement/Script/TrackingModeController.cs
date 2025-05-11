using UnityEngine;

public class TrackingModeController : MonoBehaviour
{
    private bool trackX = true;
    private bool trackZ = false;
    
    // 추적 모드 변경 이벤트
    public event System.Action<bool, bool> TrackingModeChanged;
    
    // 회전 각도에 따라 추적 모드 업데이트
    public void UpdateTrackingMode(float rotationAngle)
    {
        // 0~360 정규화
        float normalizedAngle = ((int)rotationAngle) % 360;
        if (normalizedAngle < 0) normalizedAngle += 360;
        
        bool newTrackX, newTrackZ;
        
        // 각도에 따라 분기
        if (normalizedAngle == 0 || normalizedAngle == 180)
        {
            // X축 추적
            newTrackX = true;
            newTrackZ = false;
        }
        else if (normalizedAngle == 90 || normalizedAngle == 270)
        {
            // Z축 추적
            newTrackX = false;
            newTrackZ = true;
        }
        else
        {
            // 기본값
            newTrackX = true;
            newTrackZ = false;
        }
        
        // 변경 사항이 있을 때만 이벤트 발생
        if (newTrackX != trackX || newTrackZ != trackZ)
        {
            trackX = newTrackX;
            trackZ = newTrackZ;
            
            TrackingModeChanged?.Invoke(trackX, trackZ);
            Debug.Log($"추적 모드 변경: X={trackX}, Z={trackZ} (회전 각도: {normalizedAngle}°)");
        }
    }
}