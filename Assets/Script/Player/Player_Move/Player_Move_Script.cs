using UnityEngine;
using UnityEngine.InputSystem; // 새 Input System 패키지 사용
// 집가면 리팩토링 시도함
public class Player_Move_Script : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5f; // 이동 속도
    [SerializeField] private bool faceDirectionOfMovement = true; // 이동 방향에 따라 캐릭터 회전 여부
    [SerializeField] private float gravityMultiplier = 1f; // 중력 배율
    [SerializeField] private Transform cameraTransform; // 메인 카메라 참조

    private Rigidbody rb;
    private float moveDirection = 0f;
    private bool isGrounded;
    private float gravity;
    
    // 키보드 입력을 위한 참조
    private Keyboard keyboard;

    void Start()
    {
        // 키보드 참조 가져오기
        keyboard = Keyboard.current;
        if (keyboard == null)
        {
            Debug.LogError("키보드를 찾을 수 없습니다!");
            return;
        }
        
        // 카메라 참조 가져오기 (지정되지 않은 경우)
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
            else
            {
                Debug.LogWarning("메인 카메라를 찾을 수 없습니다. 카메라 기준 이동이 작동하지 않을 수 있습니다.");
            }
        }
        
        // Rigidbody 컴포넌트 가져오기
        rb = GetComponent<Rigidbody>();
        
        // CharacterController 제거 (둘 다 있으면 충돌 발생)
        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null)
        {
            Destroy(controller);
        }
        
        // Rigidbody 설정
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Rigidbody 물리 특성 조정
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | 
                         RigidbodyConstraints.FreezeRotationX | 
                         RigidbodyConstraints.FreezeRotationY | 
                         RigidbodyConstraints.FreezeRotationZ;
        
        // 추가: 물리 특성 조정으로 튀는 현상 방지
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 충돌 감지 정밀도 높임
        rb.interpolation = RigidbodyInterpolation.Interpolate; // 움직임 부드럽게
        rb.mass = 10f; // 질량 증가
        rb.linearDamping = 5f; // 공기 저항 증가
        rb.useGravity = true; // 중력 적용
        
        // 기본 중력값 설정
        gravity = Physics.gravity.y * gravityMultiplier;
        
        // 초기에 캐릭터가 카메라를 향하도록 회전
        transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    void Update()
    {
        if (keyboard == null) return;
        
        // 키 입력 감지
        moveDirection = 0f;
        
        // 왼쪽 이동 (A 키)
        if (keyboard.aKey.isPressed)
        {
            moveDirection = -1f;
            Debug.Log("왼쪽 이동 키 입력됨");
        }
        
        // 오른쪽 이동 (D 키)
        if (keyboard.dKey.isPressed)
        {
            moveDirection = 1f;
            Debug.Log("오른쪽 이동 키 입력됨");
        }
        
        // 바닥 체크
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);
        Debug.DrawRay(transform.position, Vector3.down * 0.1f, Color.red);
    }
    
    void FixedUpdate()
    {
        if (rb == null) return;
        
        // 카메라 기준으로 이동 방향 계산
        Vector3 moveVector = CalculateMoveVector();
        
        // 현재 속도 가져오기
        Vector3 velocity = rb.linearVelocity;
        
        // 계산된 방향으로 이동 (Y속도는 유지하여 중력 적용)
        velocity.x = moveVector.x;
        velocity.z = moveVector.z;
        
        // 적용
        rb.linearVelocity = velocity;
        
        // 이동 방향에 따른 캐릭터 회전
        if (faceDirectionOfMovement && moveDirection != 0)
        {
            RotateBasedOnCamera();
        }
        
        // 디버깅 - 속도 확인
        //Debug.Log("캐릭터 속도: " + rb.linearVelocity);
    }
    
    // 카메라 기준으로 이동 벡터 계산
    private Vector3 CalculateMoveVector()
    {
        if (moveDirection == 0 || cameraTransform == null)
            return Vector3.zero;
        
        // 카메라의 오른쪽 벡터 (로컬 X축)를 기준으로 이동
        Vector3 cameraRight = cameraTransform.right;
        
        // Y축 영향 제거 (수평 이동만)
        cameraRight.y = 0;
        cameraRight.Normalize();
        
        // 이동 벡터 계산
        return cameraRight * moveDirection * moveSpeed;
    }
    
    // 카메라 기준으로 캐릭터 회전
    private void RotateBasedOnCamera()
    {
        if (cameraTransform == null) return;
        
        // 카메라가 바라보는 방향 (Y축만 사용)
        float cameraYRotation = cameraTransform.eulerAngles.y;
        
        // 이동 방향에 따라 캐릭터 회전 조정
        float characterRotation = moveDirection > 0 ? 
            cameraYRotation + 90 : // 오른쪽으로 이동 시
            cameraYRotation - 90;  // 왼쪽으로 이동 시
        
        // 회전 적용
        transform.rotation = Quaternion.Euler(0, characterRotation, 0);
    }
}
