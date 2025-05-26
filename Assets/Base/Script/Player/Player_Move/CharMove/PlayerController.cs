using UnityEngine;

[RequireComponent(typeof(PlayerPhysicsSetup))]
[RequireComponent(typeof(RigidbodyStepUp))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool faceDirectionOfMovement = true;
    
    private PlayerInputHandler inputHandler;
    private PlayerMovementController movementController;
    private PlayerAnimationController animationController;
    private PlayerRotationHandler rotationHandler;
    private RigidbodyStepUp stepUpHandler;

    void Start()
    {
        // 필요한 컴포넌트들 가져오기
        inputHandler = GetComponent<PlayerInputHandler>();
        if (inputHandler == null)
            inputHandler = gameObject.AddComponent<PlayerInputHandler>();
            
        movementController = GetComponent<PlayerMovementController>();
        if (movementController == null)
            movementController = gameObject.AddComponent<PlayerMovementController>();
            
        animationController = GetComponent<PlayerAnimationController>();
        if (animationController == null)
            animationController = gameObject.AddComponent<PlayerAnimationController>();
              rotationHandler = GetComponent<PlayerRotationHandler>();
        if (rotationHandler == null)
            rotationHandler = gameObject.AddComponent<PlayerRotationHandler>();
            
        // StepUp 컴포넌트 가져오기 (또는 추가)
        stepUpHandler = GetComponent<RigidbodyStepUp>();
        if (stepUpHandler == null)
            stepUpHandler = gameObject.AddComponent<RigidbodyStepUp>();
    }

    void Update()
    {
        // 애니메이션 상태 업데이트
        animationController.UpdateAnimationState(inputHandler.IsMoving());
    }
    
    void FixedUpdate()
    {
        float moveDirection = inputHandler.GetMoveDirection();
        float verticalInput = Input.GetAxis("Vertical");  // W/S 입력 감지
        
        // 이동 처리
        movementController.Move(moveDirection, verticalInput);
        
        // 이동 방향에 따른 캐릭터 회전
        if (faceDirectionOfMovement && moveDirection != 0)
        {
            rotationHandler.RotateBasedOnDirection(moveDirection);
        }
    }
}