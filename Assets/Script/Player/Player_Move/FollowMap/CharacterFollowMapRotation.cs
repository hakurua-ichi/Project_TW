using UnityEngine;

public class CharacterFollowMapRotation : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private string blockTag = "RotatingBlock"; // 블록에 사용할 태그

    // 유틸리티 클래스들
    private BlockCollisionDetector collisionDetector;
    private BlockPositionCalculator positionCalculator;
    private RotationPhysicsHandler physicsHandler;
    
    // 현재 상호작용 중인 회전 맵
    private RotateMap currentRotationMap;
    private bool hasRotatingMap = false;
    
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // 유틸리티 클래스 초기화
        collisionDetector = new BlockCollisionDetector(blockTag);
        collisionDetector.OnNewBlockDetected += HandleNewBlockDetected;
        positionCalculator = new BlockPositionCalculator();
        physicsHandler = new RotationPhysicsHandler(rb);
    }
    
    // 새 블록 감지 시 호출
    private void HandleNewBlockDetected(Transform block)
    {
        // 블록에서 RotateMap 컴포넌트 찾기
        RotateMap blockRotateMap = block.GetComponent<RotateMap>();
        
        // 컴포넌트가 없으면 부모 관계에서 찾기 (태그 체크 없이)
        if (blockRotateMap == null)
        {
            Transform parent = block;
            while (parent != null)
            {
                blockRotateMap = parent.GetComponent<RotateMap>();
                if (blockRotateMap != null) break;
                
                if (parent.parent == null) break;
                parent = parent.parent;
            }
        }
        
        // 발견된 RotateMap 설정
        if (blockRotateMap != null)
        {
            currentRotationMap = blockRotateMap;
            hasRotatingMap = true;
            //Debug.Log($"[회전 맵] 회전 맵 자동 감지: {currentRotationMap.name}");
        }
    }
    
    void Update()
    {
        // 현재 회전 맵이 없으면 건너뛰기
        if (!hasRotatingMap || currentRotationMap == null)
        {
            // 물리 처리 정상화
            physicsHandler.HandlePhysicsForRotation(false);
            return;
        }
        
        bool isRotating = currentRotationMap.IsRotating();
        
        // 맵 회전 시작/종료 감지 및 물리 처리
        physicsHandler.HandlePhysicsForRotation(isRotating);
        
        // 맵이 회전 중일 때만 블록 중앙으로 이동
        if (isRotating && collisionDetector.CurrentBlock != null)
        {
            Vector3 targetPosition = positionCalculator.CalculatePositionAboveBlock(collisionDetector.CurrentBlock);
            transform.position = targetPosition;
        }
    }
    
    // 충돌 감지 함수들
    private void OnCollisionEnter(Collision collision) => collisionDetector.CheckCollision(collision);
    private void OnCollisionStay(Collision collision) => collisionDetector.CheckCollision(collision);
    
    private void OnCollisionExit(Collision collision)
    {
        bool isRotating = hasRotatingMap && currentRotationMap != null && currentRotationMap.IsRotating();
        collisionDetector.ExitCollision(collision.transform, isRotating);
        
        // 현재 블록과의 충돌이 종료되고 회전 중이 아니면 회전 맵 참조 해제
        if (collision.transform == collisionDetector.CurrentBlock && !isRotating)
        {
            currentRotationMap = null;
            hasRotatingMap = false;
        }
    }
    
    private void OnDisable()
    {
        if (collisionDetector != null)
        {
            collisionDetector.OnNewBlockDetected -= HandleNewBlockDetected;
            collisionDetector.Reset();
        }
    }
}