using UnityEngine;

public class PlayerPhysicsSetup : MonoBehaviour
{
    [Header("캐릭터 컨트롤러 설정")]
    [SerializeField] private float skinWidth = 0.08f;
    [SerializeField] private float slopeLimit = 45f;
    [SerializeField] private float stepOffset = 0.3f;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private float height = 2f;
    [SerializeField] private Vector3 center = new Vector3(0, 1f, 0);
    
    private CharacterController characterController;

    void Start()
    {
        // CharacterController 컴포넌트 가져오기
        characterController = GetComponent<CharacterController>();
        
        // CharacterController 설정
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
        }

        // CharacterController 물리 특성 조정
        characterController.skinWidth = skinWidth;
        characterController.slopeLimit = slopeLimit;
        characterController.stepOffset = stepOffset;
        characterController.radius = radius;
        characterController.height = height;
        characterController.center = center;
        
        // Rigidbody가 있는 경우 제거 (CharacterController와 호환되지 않음)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Debug.Log("CharacterController 사용을 위해 Rigidbody 컴포넌트를 제거합니다.");
            Destroy(rb);
        }
    }
}