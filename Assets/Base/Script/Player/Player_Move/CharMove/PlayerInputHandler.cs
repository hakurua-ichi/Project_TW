using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private float moveDirection = 0f;
    private bool isMoving = false;
    private CharacterController controller;

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

        controller = GetComponent<CharacterController>();
    }
    
    void Update()
    {
        // 키 입력 감지 (터치 컨트롤이 활성화된 경우 스킵)
        if (!isTouchControlActive)
        {
            moveDirection = 0f;
            isMoving = false;
            
            // 조이스틱 입력 감지
            if (joystick != null && joystick.IsMoving())
            {
                moveDirection = joystick.GetHorizontal();
                isMoving = true;
            }
            // 키보드 입력 감지
            else
            { 
                // 왼쪽 이동 (A 키)
                if (Input.GetKey(KeyCode.A))
                {
                    moveDirection = -1f;
                    isMoving = true;
                }
                
                // 오른쪽 이동 (D 키)
                if (Input.GetKey(KeyCode.D))
                {
                    moveDirection = 1f;
                    isMoving = true;
                }
                
                // 점프 처리 (스페이스바)
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    PlayerMovementController movementController = GetComponent<PlayerMovementController>();
                    if (movementController != null)
                    {
                        movementController.Jump();
                    }
                }
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
    
    // 터치 UI를 통한 이동 방향 설정
    private float touchMoveDirection = 0f;
    private bool isTouchControlActive = false;
    
    // TouchUIScript에서 호출하는 메서드
    public void SetTouchMoveDirection(float direction)
    {
        touchMoveDirection = direction;
        isTouchControlActive = direction != 0f;
        
        // 즉시 이동 반영
        if (isTouchControlActive)
        {
            moveDirection = touchMoveDirection;
            isMoving = true;
        }
        // 터치 컨트롤이 해제되었을 때만 움직임 멈춤 (키보드 입력이 있으면 그대로 유지)
        else if (Mathf.Approximately(moveDirection, touchMoveDirection))
        {
            moveDirection = 0f;
            isMoving = false;
        }
    }
}