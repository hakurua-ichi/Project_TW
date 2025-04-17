using UnityEngine;

public class RotationInput : MonoBehaviour
{
    // 회전 이벤트 (시계/반시계)
    public event System.Action<float> RotationRequested;
    
    void Update()
    {
        // 회전 입력 감지
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotationRequested?.Invoke(90f);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            RotationRequested?.Invoke(-90f);
        }
    }
}