using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(BoxCollider))]
public class BackfaceOccludeController : MonoBehaviour
{
    public Transform target;    // 플레이어 or 카메라
    private Material originalMaterial;    // 원본 백페이스 오클루더 머티리얼
    private bool isPlayerInside = false;  // 플레이어가 박스 내부에 있는지 여부
    private bool isDetectedByWallDetector = false; // 카메라 WallDetector에 의한 감지 여부
    
    private Renderer objectRenderer;
    private Material[] originalMaterials;  // 보관용 원본 머티리얼 배열
    [SerializeField] private Material transparentMaterial; // 스텐실 투명 머티리얼 할당
    
    // 외부에서 접근 가능한 속성
    public bool IsPlayerInside { get { return isPlayerInside; } }
    
    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        
        // 원본 재질 저장 (반드시 sharedMaterials 사용)
        if (objectRenderer != null) {
            originalMaterial = objectRenderer.material;  // 백페이스 오클루더 머티리얼
            originalMaterials = objectRenderer.sharedMaterials;  // 원본 머티리얼 배열 복사
        } else {
            Debug.LogError($"No Renderer found on {gameObject.name}!", this);
            return;
        }
        
        // BoxCollider 설정
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.isTrigger = true;
        }
        else
        {
            Debug.LogError("BoxCollider가 필요합니다. 자동으로 추가되어야 합니다.");
        }
        
        // 투명 머티리얼이 지정되지 않았다면 경고
        if (transparentMaterial == null) {
            Debug.LogWarning("투명 머티리얼이 지정되지 않았습니다. Inspector에서 설정해주세요.", this);
        }
    }
    
    void Update()
    {
        if (originalMaterial != null && target != null)
        {
            // 타겟 위치를 셰이더에 전달
            originalMaterial.SetVector("_TargetPos", target.position);
            
            // 플레이어의 내/외부 상태를 셰이더에 전달
            originalMaterial.SetFloat("_IsInside", isPlayerInside ? 1.0f : 0.0f);
            
            // 시각화 관리
            UpdateVisibility();
        }
    }
    
    // 플레이어가 박스 내부로 들어왔을 때 호출
    void OnTriggerEnter(Collider other)
    {
        // 플레이어 태그를 가진 객체가 들어왔는지 확인
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            Debug.Log($"{gameObject.name}: 플레이어가 박스 내부로 들어왔습니다.");
        }
    }
    
    // 플레이어가 박스 내부에서 나갔을 때 호출
    void OnTriggerExit(Collider other)
    {
        // 플레이어 태그를 가진 객체가 나갔는지 확인
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            Debug.Log($"{gameObject.name}: 플레이어가 박스 외부로 나갔습니다.");
            
            // 명시적으로 머티리얼 복원 - 무조건 원본으로 복원
            if (!isDetectedByWallDetector) {
                ApplyOriginalMaterials();
            }
        }
    }
    
    // WallDetector에 의한 감지 상태 설정 메서드 (BoxOccluderManager에서 호출)
    public void SetDetectedByWallDetector(bool isDetected)
    {
        bool previousState = isDetectedByWallDetector;
        isDetectedByWallDetector = isDetected;
        
        // 상태가 변경된 경우에만 로그 출력
        if (previousState != isDetectedByWallDetector) {
            Debug.Log($"{gameObject.name}: WallDetector 감지 상태 변경 - {isDetectedByWallDetector}");
        }
    }
    
    // 시각화 업데이트
    private void UpdateVisibility()
    {
        if (objectRenderer == null) return;
        
        if (isPlayerInside)
        {
            // 플레이어가 내부에 있으면 BackfaceOccluder 셰이더 사용
            ApplyOriginalMaterials();
        }
        else if (isDetectedByWallDetector && transparentMaterial != null)
        {
            // 플레이어가 외부에 있고 WallDetector에 감지되면 투명 머티리얼 적용
            ApplyTransparentMaterial();
        }
        else
        {
            // 그 외 경우 원본 재질 사용
            ApplyOriginalMaterials();
        }
    }
    
    // 투명 머티리얼 적용
    private void ApplyTransparentMaterial()
    {
        if (objectRenderer == null || transparentMaterial == null) return;
        
        Material[] newMats = new Material[originalMaterials.Length];
        for (int i = 0; i < newMats.Length; i++) {
            newMats[i] = transparentMaterial;
        }
        
        objectRenderer.sharedMaterials = newMats;
    }
    
    // 원본 머티리얼 적용
    private void ApplyOriginalMaterials()
    {
        if (objectRenderer == null || originalMaterials == null) return;
        
        // 원본 머티리얼 배열을 복사하여 적용
        Material[] restoreMats = new Material[originalMaterials.Length];
        for (int i = 0; i < originalMaterials.Length; i++) {
            restoreMats[i] = originalMaterials[i];
        }
        
        objectRenderer.sharedMaterials = restoreMats;
    }
    
    // 컴포넌트가 비활성화될 때 원본 머티리얼 복원
    void OnDisable()
    {
        if (objectRenderer != null && originalMaterials != null) {
            objectRenderer.sharedMaterials = originalMaterials;
        }
    }
    
    // 컴포넌트가 소멸될 때 원본 머티리얼 복원
    void OnDestroy()
    {
        if (objectRenderer != null && originalMaterials != null) {
            objectRenderer.sharedMaterials = originalMaterials;
        }
    }
}