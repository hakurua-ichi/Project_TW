using UnityEngine;

[RequireComponent(typeof(PlayerPhysicsSetup))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool faceDirectionOfMovement = true;
    
    private PlayerInputHandler inputHandler;
    private PlayerMovementController movementController;
    private PlayerAnimationController animationController;
    private PlayerRotationHandler rotationHandler;

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
    }

    void Update()
    {
        // 애니메이션 상태 업데이트
        animationController.UpdateAnimationState(inputHandler.IsMoving());
    }
    
    void FixedUpdate()
    {
        float moveDirection = inputHandler.GetMoveDirection();
        
        // 이동 처리
        movementController.Move(moveDirection,0f);
        
        // 이동 방향에 따른 캐릭터 회전
        if (faceDirectionOfMovement && moveDirection != 0)
        {
            rotationHandler.RotateBasedOnDirection(moveDirection);
        }
    }
}