using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private float moveDirection = 0f;
    private bool isMoving = false;
    private Rigidbody rb;

    [SerializeField] private JoystickController joystick;
    
    void Start()
    {
        if (joystick == null)
        {
            joystick = FindAnyObjectByType<JoystickController>();
            if (joystick == null)
            {
                Debug.LogWarning("조이스틱을 찾을 수 없습니다. 조이스틱 컨트롤러가 씬에 없거나 비활성화되어 있습니다.");
                Canvas UIcanvas = FindAnyObjectByType<Canvas>();
            }
            if (Camera.main && Camera.main.GetComponent<CameraSystemController>() == null)
            {
                Debug.LogWarning("카메라 시스템 컨트롤러를 찾을 수 없습니다. 카메라 시스템이 씬에 없거나 비활성화되어 있습니다.");
            }
        }

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 키 입력 감지
        moveDirection = 0f;
        isMoving = false;
        
        // 조이스틱 입력 감지
        if (joystick != null && joystick.IsMoving() && !rb.isKinematic)
        {
            moveDirection = joystick.GetHorizontal();
            isMoving = true;
        }
        // 키보드 입력 감지
        else if (!rb.isKinematic)
        { 
            // 왼쪽 이동 (A 키)
            if (Input.GetKey(KeyCode.A) && rb != null)
            {
                moveDirection = -1f;
                isMoving = true;
            }
            
            // 오른쪽 이동 (D 키)
            if (Input.GetKey(KeyCode.D) && rb != null)
            {
                moveDirection = 1f;
                isMoving = true;
            }
        }
    }

    public float GetMoveDirection()
    {
        return moveDirection;
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}