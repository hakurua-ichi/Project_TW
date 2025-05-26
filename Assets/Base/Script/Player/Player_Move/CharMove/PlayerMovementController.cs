using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravityMultiplier = 2.5f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rotationSpeed = 10f;    private CharacterController controller;
    private bool isGrounded;
    private float verticalVelocity = 0f;
    private float airControl = 0.3f;
    
    // Character Controller용 이동 제한 속성 (Rigidbody.isKinematic 대체)
    private bool canMove = true;

    void Start()
    {
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
            else
            {
                Debug.LogWarning("메인 카메라를 찾을 수 없습니다.");
            }
        }

        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }
    }

    void Update()
    {
        // CharacterController로 지면 감지
        isGrounded = controller.isGrounded;
    }    public void Move(float horizontalInput, float verticalInput)
    {
        if (controller == null) return;

        // 이동 제한 상태 체크 (isKinematic 대체)
        if (!canMove)
        {
            // 이동 불가 상태일 때는 중력만 적용
            ApplyGravity();
            controller.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
            return;
        }

        if (verticalInput != 0)
        {
            RotateTowardCamera(verticalInput);
        }

        Vector3 moveVector = CalculateMoveVector(horizontalInput, verticalInput);

        // 중력 적용
        ApplyGravity();

        // 최종 이동 벡터 생성 (수평 이동 + 수직 속도)
        Vector3 finalMoveVector = new Vector3(moveVector.x, verticalVelocity, moveVector.z);

        // CharacterController를 사용하여 이동
        controller.Move(finalMoveVector * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        // 지면에 있을 때
        if (isGrounded && verticalVelocity < 0)
        {
            // 약간의 하향력 유지 (지면에 붙어있도록)
            verticalVelocity = -0.5f;
        }
        // 공중에 있을 때
        else
        {
            // 중력 적용
            verticalVelocity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
    }

    public void Jump()
    {
        if (isGrounded)
        {
            verticalVelocity = jumpForce;
        }
    }

    private void RotateTowardCamera(float verticalInput)
    {
        if (cameraTransform == null) return;

        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 targetDirection = (verticalInput > 0) ? cameraForward : -cameraForward;

        if (targetDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private Vector3 CalculateMoveVector(float horizontalInput, float verticalInput)
    {
        if ((horizontalInput == 0 && verticalInput == 0) || cameraTransform == null)
            return Vector3.zero;

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        return (cameraRight * horizontalInput + cameraForward * verticalInput) * moveSpeed;
    }    public bool IsGrounded()
    {
        return isGrounded;
    }
    
    // isKinematic 대체 메서드 - 이동 가능 여부 설정
    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
        
        if (!enabled)
        {
            // 이동 불가 상태로 전환 시 속도 초기화
            verticalVelocity = 0f;
        }
    }
    
    // 현재 이동 가능 상태 확인
    public bool CanMove()
    {
        return canMove;
    }
}
