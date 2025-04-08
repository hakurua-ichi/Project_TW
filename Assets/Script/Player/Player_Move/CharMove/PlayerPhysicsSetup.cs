using UnityEngine;

public class PlayerPhysicsSetup : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
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
        rb.constraints = RigidbodyConstraints.FreezePositionZ | 
                        RigidbodyConstraints.FreezeRotationX | 
                        RigidbodyConstraints.FreezeRotationY | 
                        RigidbodyConstraints.FreezeRotationZ;
        
        // 물리 특성 조정으로 튀는 현상 방지
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.mass = 10f;
        rb.linearDamping = 5f;
        rb.useGravity = true;
    }
}