using UnityEngine;

public class PlayerPositionTracker : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float followSpeed = 4f;
    [SerializeField] private float offsetThreshold = 2f;
    
    // Y축 오프셋 값 추가
    [SerializeField] private float heightOffset = 2f;
    [SerializeField] private bool trackHeight = false; // Y축 추적 여부
    
    private bool trackX = true;
    private bool trackZ = false;
    
    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }
    
    public void SetTrackingMode(bool x, bool z)
    {
        trackX = x;
        trackZ = z;
    }
    
    public void UpdatePosition()
    {
        if (playerTransform == null) return;
        
        Vector3 currentPos = transform.position;
        Vector3 playerPos = playerTransform.position;
        Vector3 newPos = currentPos;
        
        // X축 추적
        if (trackX && Mathf.Abs(playerPos.x - currentPos.x) > offsetThreshold)
        {
            newPos.x = Mathf.Lerp(currentPos.x, playerPos.x, Time.deltaTime * followSpeed);
        }
        
        // Z축 추적
        if (trackZ && Mathf.Abs(playerPos.z - currentPos.z) > offsetThreshold)
        {
            newPos.z = Mathf.Lerp(currentPos.z, playerPos.z, Time.deltaTime * followSpeed);
        }
        
        // Y축 추적 (heightOffset 적용)
        if (trackHeight && Mathf.Abs((playerPos.y + heightOffset) - currentPos.y) > offsetThreshold)
        {
            newPos.y = Mathf.Lerp(currentPos.y, playerPos.y + heightOffset, Time.deltaTime * followSpeed);
        }
        else
        {
            // 추적하지 않을 경우 현재 높이 유지
            newPos.y = currentPos.y;
        }
        
        transform.position = newPos;
    }
    
    // Y축 추적 활성화/비활성화 메서드 (선택적)
    public void SetHeightTracking(bool track)
    {
        trackHeight = track;
    }
    
    // Y축 오프셋 설정 메서드 (선택적)
    public void SetHeightOffset(float offset)
    {
        heightOffset = offset;
    }
}