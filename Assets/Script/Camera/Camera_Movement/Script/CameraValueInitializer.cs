using UnityEngine;

public class CameraValueInitializer : MonoBehaviour
{
    private float distance;
    private float height;
    private float angle;
    
    // 초기화 완료 이벤트
    public event System.Action<float, float, float> InitializationCompleted;
    
    public void Initialize(Transform camera, Transform player)
    {
        if (camera == null || player == null) return;
        
        Vector3 offset = camera.position - player.position;
        
        // 값 계산
        height = offset.y;
        distance = new Vector3(offset.x, 0, offset.z).magnitude;
        angle = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
        
        // 이벤트 발생
        InitializationCompleted?.Invoke(distance, height, angle);
        
        Debug.Log($"카메라 초기값: 거리={distance}, 높이={height}, 각도={angle}°");
    }
}