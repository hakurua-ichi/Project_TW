using UnityEngine;

public class PlayerRotationHandler : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    void Start()
    {
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
        }
        
        // 초기에 캐릭터가 카메라를 향하도록 회전
        transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    public void RotateBasedOnDirection(float direction)
    {
        if (cameraTransform == null || direction == 0) return;
        
        // 카메라가 바라보는 방향 (Y축만 사용)
        float cameraYRotation = cameraTransform.eulerAngles.y;
        
        // 이동 방향에 따라 캐릭터 회전 조정
        float characterRotation = direction > 0 ? 
            cameraYRotation + 90 : // 오른쪽으로 이동 시
            cameraYRotation - 90;  // 왼쪽으로 이동 시
        
        // 회전 적용
        transform.rotation = Quaternion.Euler(0, characterRotation, 0);
    }
}