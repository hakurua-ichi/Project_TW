using UnityEngine;

[RequireComponent(typeof(PlayerPhysicsSetup))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerMovementController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool faceDirectionOfMovement = true;

    private PlayerInputHandler inputHandler;
    private PlayerMovementController movementController;
    private PlayerAnimationController animationController;
    private PlayerRotationHandler rotationHandler;
    private CharacterController characterController;

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
        
        // CharacterController 컴포넌트 참조 설정
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
            characterController = gameObject.AddComponent<CharacterController>();        // 지면 스냅핑을 위한 컴포넌트 추가 (없는 경우)
        if (GetComponent<CharacterGroundSnapper>() == null)
        {
            gameObject.AddComponent<CharacterGroundSnapper>();
        }
        
        // 맵 회전 추적 컴포넌트 추가 (없는 경우)
        if (GetComponent<CharacterMapFollower>() == null)
        {
            gameObject.AddComponent<CharacterMapFollower>();
        }

        // 컴포넌트 유효성 검사
        if (characterController == null || inputHandler == null || movementController == null || rotationHandler == null)
        {
            Debug.LogError("PlayerController에 필요한 컴포넌트가 없습니다!");
            enabled = false;
        }
    }

    void Update()
    {
        // 애니메이션 상태 업데이트
        animationController.UpdateAnimationState(inputHandler.IsMoving());
        
        // 입력 처리
        float moveDirection = inputHandler.GetMoveDirection();
        float verticalInput = Input.GetAxis("Vertical");  // W/S 입력 감지
        bool isMoving = inputHandler.IsMoving();

        // 이동 처리
        if (isMoving)
        {
            movementController.Move(moveDirection, verticalInput);
            
            // 이동 방향에 따른 캐릭터 회전
            if (faceDirectionOfMovement && moveDirection != 0)
            {
                rotationHandler.RotateBasedOnDirection(moveDirection);
            }
        }
        else
        {
            // 이동하지 않을 때도 중력 등 적용
            movementController.Move(0, 0);
        }
    }
    
    // CharacterController 충돌 이벤트 처리
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 필요한 경우 충돌 처리 로직 구현
    }
}