using UnityEngine;

public class RotationInput : MonoBehaviour
{
    // 회전 이벤트 (시계/반시계)
    // 구독자:
    // - CameraSystemController.OnRotationRequested(float angle)
    //   위치: Assets\Script\Camera\Camera_Movement\Script\CameraSystemController.cs
    public event System.Action<float> RotationRequested;
    
    // TouchUIScript에서 호출 가능한 공개 메서드
    public void RequestRotation(float angle)
    {
        // RotationRequested 이벤트 발생
        RotationRequested?.Invoke(angle);
    }
    
    void Update()
    {
        // 회전 입력 감지
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 시계 방향 회전 요청 (90도)
            // 구독자:
            // - CameraSystemController.OnRotationRequested(float angle)
            //   위치: Assets\Script\Camera\Camera_Movement\Script\CameraSystemController.cs
            RotationRequested?.Invoke(90f);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            // 반시계 방향 회전 요청 (-90도)
            // 구독자:
            // - CameraSystemController.OnRotationRequested(float angle)
            //   위치: Assets\Script\Camera\Camera_Movement\Script\CameraSystemController.cs
            RotationRequested?.Invoke(-90f);
        }
    }
}