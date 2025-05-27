using UnityEngine;
using System.Collections;

/// <summary>
/// CharacterFollowMapRotation
/// 
/// 캐릭터가 회전하는 맵 블록을 따라 회전하고, 지면에 고정되도록 처리하는 컴포넌트
/// 맵이나 카메라 회전 시 캐릭터 이동을 제한하고, 위치를 블록 위에 고정시킴
/// 
/// 주요 기능:
/// - 블록 충돌 감지 및 상호작용
/// - 맵/카메라 회전 시 캐릭터 위치 조정
/// - 이동 제한 상태 관리
/// - 지면 스냅핑 처리
/// - 애니메이션 상태 연동
/// </summary>
public class CharacterFollowMapRotation : MonoBehaviour
{    
    [Header("설정")]
    [SerializeField] private string blockTag = "RotatingBlock"; // 블록에 사용할 태그

    // 유틸리티 클래스들
    private BlockCollisionDetector collisionDetector;
    private BlockPositionCalculator positionCalculator;
    private CharacterController controller;
    private RotationInterpolator cameraRotationInterpolator;
    
    // 현재 상호작용 중인 회전 맵
    private RotateMap currentRotationMap;
    private bool hasRotatingMap = false;
    
    // 카메라 회전 관련
    private bool wasCameraRotating = false;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraRotationInterpolator = Camera.main.GetComponent<RotationInterpolator>();
        
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }
        
        // 유틸리티 클래스 초기화
        collisionDetector = new BlockCollisionDetector(blockTag);
        collisionDetector.OnNewBlockDetected += HandleNewBlockDetected;
        positionCalculator = new BlockPositionCalculator();
        
        // 카메라 회전 시스템과 연결
        CameraSystemController cameraSystemController = Camera.main?.GetComponent<CameraSystemController>();
        if (cameraSystemController != null)
        {
            cameraSystemController.RotationCompleted += OnCameraRotationCompleted;
        }
    }
    
    // 카메라 회전이 완료되었을 때 호출되는 메서드
    private void OnCameraRotationCompleted(float angle)
    {
        // 회전 직후 캐릭터 위치 조정 로직
        if (collisionDetector.CurrentBlock != null)
        {
            Vector3 finalPosition = positionCalculator.CalculatePositionAboveBlock(collisionDetector.CurrentBlock);
            controller.Move(finalPosition - transform.position);
            
            // 카메라 회전 후 상태 추적
            StartCoroutine(DelayedMovementEnable(0.5f));
        }
    }
    
    // 일정 시간 후에 이동을 다시 활성화하는 코루틴
    private System.Collections.IEnumerator DelayedMovementEnable(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // 다른 제한 사항이 없는 경우에만 이동 활성화
        PlayerMovementController movementController = GetComponent<PlayerMovementController>();
        if (movementController != null)
        {
            movementController.SetMovementEnabled(true);
            
            // PlayerMovementRestrictor에서도 제한 해제
            PlayerMovementRestrictor restrictor = PlayerMovementRestrictor.Instance;
            if (restrictor != null)
            {
                restrictor.AllowMovement(PlayerMovementRestrictor.RestrictionReason.CameraRotation);
            }
        }
        
        // 카메라 회전 후 플래그 초기화
        wasCameraRotating = false;
    }
    
    // 새 블록 감지 시 호출
    private void HandleNewBlockDetected(Transform block)
    {
        // 직접 검색 대신 부모까지 포함하여 검색
        RotateMap blockRotateMap = block.GetComponentInParent<RotateMap>();
        
        if (blockRotateMap != null)
        {
            currentRotationMap = blockRotateMap;
            hasRotatingMap = true;
        }
    }
    
    void Update()
    {
        // 현재 회전 맵이 없으면 건너뛰기
        if (!hasRotatingMap || currentRotationMap == null)
        {
            return;
        }
        
        // 맵 또는 카메라 회전 감지
        bool isMapRotating = currentRotationMap.IsRotating();
        bool isCameraRotating = cameraRotationInterpolator != null && cameraRotationInterpolator.IsRotating;
        bool isRotating = isMapRotating || isCameraRotating;
        
        // 회전 상태 변화 감지 및 처리
        UpdateMovementRestriction(isMapRotating, isCameraRotating || wasCameraRotating);
        
        // 맵이 회전 중이거나 카메라가 회전 중일 때만 블록 중앙으로 이동
        if ((isRotating || wasCameraRotating) && collisionDetector.CurrentBlock != null)
        {
            Vector3 targetPosition = positionCalculator.CalculatePositionAboveBlock(collisionDetector.CurrentBlock);
            
            // CharacterController의 Move 메서드 사용하여 이동 (단순 transform 위치 변경 대신)
            // 현재 위치와 목표 위치의 차이를 구해 그 차이만큼 이동
            Vector3 moveDirection = targetPosition - transform.position;
            controller.Move(moveDirection);
        }
        
        // 회전이 끝난 직후 잠시 동안 블록 위치에 고정시키기 위한 관리
        if (!isRotating && wasCameraRotating)
        {
            // 카메라 회전이 끝난 직후 상태 추적
            wasCameraRotating = false;
            
            // 회전이 끝난 직후 캐릭터가 블록 위에 정확히 위치하도록 한번 더 위치 조정
            if (collisionDetector.CurrentBlock != null)
            {
                Vector3 finalPosition = positionCalculator.CalculatePositionAboveBlock(collisionDetector.CurrentBlock);
                controller.Move(finalPosition - transform.position);
            }
        }
        else if (isCameraRotating)
        {
            // 카메라가 회전 중일 때 상태 표시
            wasCameraRotating = true;
        }
    }
    
    // 충돌 감지 함수들 - CharacterController 사용 시 OnControllerColliderHit 사용
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // RotateMap 찾기
        RotateMap blockRotateMap = hit.gameObject.GetComponentInParent<RotateMap>();
        if (blockRotateMap != null)
        {
            currentRotationMap = blockRotateMap;
            hasRotatingMap = true;
        }
        
        // 기존 충돌 로직 호출 - 태그로 블록 감지
        if (hit.gameObject.CompareTag(blockTag))
        {
            collisionDetector.CheckCollision(hit.transform);
        }
    }
    
    // 회전에 따른 이동 제한 상태 업데이트
    private void UpdateMovementRestriction(bool isMapRotating, bool isCameraRotating)
    {
        // 상태 변화 감지를 위해 PlayerMovementController 참조
        PlayerMovementController movementController = GetComponent<PlayerMovementController>();
        if (movementController == null) return;
        
        // 회전 중 또는 회전 직후인 경우 이동 제한
        if (isMapRotating || isCameraRotating)
        {
            movementController.SetMovementEnabled(false);
            
            // 애니메이션 컨트롤러 참조 및 애니메이션 멈춤 처리
            PlayerAnimationController animController = GetComponent<PlayerAnimationController>();
            if (animController != null)
            {
                animController.UpdateAnimationState(false);
            }
            
            // PlayerMovementRestrictor에도 이동 제한 이유 등록 (싱글톤 접근)
            PlayerMovementRestrictor restrictor = PlayerMovementRestrictor.Instance;
            if (restrictor != null)
            {
                if (isMapRotating)
                {
                    restrictor.RestrictMovement(PlayerMovementRestrictor.RestrictionReason.MapRotation);
                }
                if (isCameraRotating)
                {
                    restrictor.RestrictMovement(PlayerMovementRestrictor.RestrictionReason.CameraRotation);
                }
            }
        }
        // 그렇지 않으면 이동 허용
        else
        {
            movementController.SetMovementEnabled(true);
            
            // PlayerMovementRestrictor에서도 제한 해제 (싱글톤 접근)
            PlayerMovementRestrictor restrictor = PlayerMovementRestrictor.Instance;
            if (restrictor != null)
            {
                restrictor.AllowMovement(PlayerMovementRestrictor.RestrictionReason.MapRotation);
                restrictor.AllowMovement(PlayerMovementRestrictor.RestrictionReason.CameraRotation);
            }
        }
    }
    
    private void OnDisable()
    {
        if (collisionDetector != null)
        {
            collisionDetector.OnNewBlockDetected -= HandleNewBlockDetected;
            collisionDetector.Reset();
        }
        
        // 카메라 회전 이벤트 구독 해제
        CameraSystemController cameraSystemController = Camera.main?.GetComponent<CameraSystemController>();
        if (cameraSystemController != null)
        {
            cameraSystemController.RotationCompleted -= OnCameraRotationCompleted;
        }
    }
}
