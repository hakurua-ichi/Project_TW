// using UnityEngine;

// public class PlayerMovementController : MonoBehaviour
// {
//     [SerializeField] private float moveSpeed = 10f;
//     [SerializeField] private Transform cameraTransform;
//     [SerializeField] private float rotationSpeed = 10f;

//     private Rigidbody rb;
//     private bool isGrounded;
//     private float airControl = 0.3f;
    
//     // 스텝업 관련
//     private RigidbodyStepUp stepUpHandler;

//     void Start()
//     {
//         if (cameraTransform == null)
//         {
//             Camera mainCamera = Camera.main;
//             if (mainCamera != null)
//             {
//                 cameraTransform = mainCamera.transform;
//             }
//             else
//             {
//                 Debug.LogWarning("메인 카메라를 찾을 수 없습니다.");
//             }
//         }

//         rb = GetComponent<Rigidbody>();
//         if (rb == null)
//         {
//             rb = gameObject.AddComponent<Rigidbody>();
//         }
        
//         // StepUp 컴포넌트 가져오기
//         stepUpHandler = GetComponent<RigidbodyStepUp>();
//         if (stepUpHandler == null)
//         {
//             stepUpHandler = gameObject.AddComponent<RigidbodyStepUp>();
//         }
//     }

//     void Update()
//     {
//         isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);
//     }

//     public void Move(float horizontalInput, float verticalInput)
//     {
//         if (rb == null) return;

//         if (verticalInput != 0)
//         {
//             RotateTowardCamera(verticalInput);
//         }

//         Vector3 moveVector = CalculateMoveVector(horizontalInput, verticalInput);

//         Vector3 velocity = rb.velocity;

//         // 자연스러운 이동을 위해 공중에서는 부분적으로 입력 적용
//         if (!isGrounded && !rb.isKinematic)
//         {
//             velocity.x = velocity.x * (1 - airControl) + moveVector.x * airControl;
//             velocity.z = velocity.z * (1 - airControl) + moveVector.z * airControl;
//             rb.velocity = velocity;
//         }
//         // 지면에서는 직접 속도 설정
//         else if (!rb.isKinematic && isGrounded)
//         {
//             velocity.x = moveVector.x;
//             velocity.z = moveVector.z;
//             rb.velocity = velocity;
//         }
//     }

//     private void RotateTowardCamera(float verticalInput)
//     {
//         if (cameraTransform == null) return;

//         Vector3 cameraForward = cameraTransform.forward;
//         cameraForward.y = 0;
//         cameraForward.Normalize();

//         Vector3 targetDirection = (verticalInput > 0) ? cameraForward : -cameraForward;

//         if (targetDirection.sqrMagnitude > 0.001f)
//         {
//             Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
//             transform.rotation = Quaternion.Slerp(
//                 transform.rotation,
//                 targetRotation,
//                 rotationSpeed * Time.deltaTime
//             );
//         }
//     }

//     private Vector3 CalculateMoveVector(float horizontalInput, float verticalInput)
//     {
//         if ((horizontalInput == 0 && verticalInput == 0) || cameraTransform == null)
//             return Vector3.zero;

//         Vector3 cameraForward = cameraTransform.forward;
//         Vector3 cameraRight = cameraTransform.right;

//         cameraForward.y = 0;
//         cameraRight.y = 0;
//         cameraForward.Normalize();
//         cameraRight.Normalize();

//         return (cameraRight * horizontalInput + cameraForward * verticalInput) * moveSpeed;
//     }

//     public bool IsGrounded()
//     {
//         return isGrounded;
//     }
// }
