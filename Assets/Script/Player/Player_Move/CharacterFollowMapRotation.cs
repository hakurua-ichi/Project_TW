using UnityEngine;

public class CharacterFollowMapRotation : MonoBehaviour
{
    [Header("회전축 부모")]
    public RotateMap rotationParent;

    [Header("설정")]
    [SerializeField] private string blockTag = "RotatingBlock"; // 블록에 사용할 태그

    // 유틸리티 클래스들
    private BlockCollisionDetector collisionDetector;
    private BlockPositionCalculator positionCalculator;
    private RotationPhysicsHandler physicsHandler;
    
    private Rigidbody rb;

    void Start()
    {
        if (rotationParent == null)
        {
            Debug.LogError("[회전 맵] 회전축 부모가 할당되지 않았습니다.");
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody>();
        
        // 유틸리티 클래스 초기화
        collisionDetector = new BlockCollisionDetector(blockTag);
        positionCalculator = new BlockPositionCalculator();
        physicsHandler = new RotationPhysicsHandler(rb);
        
        // 이벤트 구독
        collisionDetector.OnNewBlockDetected += block => Debug.Log($"[회전 맵] 새 블록 감지: {block.name}");
    }
    
    void Update()
    {
        if (rotationParent == null) return;
        
        bool isRotating = rotationParent.IsRotating();
        
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
        collisionDetector.ExitCollision(collision.transform, rotationParent.IsRotating());
    }
    
    private void OnDisable()
    {
        collisionDetector.Reset();
    }
}