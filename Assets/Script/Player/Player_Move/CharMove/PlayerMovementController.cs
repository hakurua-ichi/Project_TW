using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private Transform cameraTransform;
    
    private Rigidbody rb;
    private bool isGrounded;
    private float airControl = 0.3f;

    void Start()
    {
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
        
        // Rigidbody 설정
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
    }

    void Update()
    {
        // 바닥 체크
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);
    }

    public void Move(float horizontalInput, float verticalInput)
    {
        if (rb == null) return;
        
        // 카메라 기준으로 이동 방향 계산
        Vector3 moveVector = CalculateMoveVector(horizontalInput, verticalInput);
        
        // 현재 속도 가져오기
        Vector3 velocity = rb.linearVelocity;
        
        // 계산된 방향으로 이동 (Y속도는 유지하여 중력 적용)
        if(!isGrounded && !rb.isKinematic)
        {
            velocity.x = velocity.x * (1 - airControl) + moveVector.x * airControl;
            velocity.z = velocity.z * (1 - airControl) + moveVector.z * airControl;
            rb.linearVelocity = velocity;
        }
        if(!rb.isKinematic && isGrounded)
        {
            velocity.x = moveVector.x;
            velocity.z = moveVector.z;
            rb.linearVelocity = velocity;
        }
        
    }

    private Vector3 CalculateMoveVector(float horizontalInput, float verticalInput)
    {
        if ((horizontalInput == 0 && verticalInput == 0) || cameraTransform == null)
            return Vector3.zero;
        
        // 카메라의 방향 벡터 가져오기
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        
        // Y축 영향 제거 (수평 이동만)
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        // 이동 벡터 계산
        return (cameraRight * horizontalInput + cameraForward * verticalInput) * moveSpeed;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}