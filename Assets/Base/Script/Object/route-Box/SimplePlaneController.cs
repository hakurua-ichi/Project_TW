using UnityEngine;
using System.Collections.Generic;

public class SimplePlaneGroupController : MonoBehaviour
{
    [Tooltip("카메라 WallDetector에 감지될 때 사용할 투명 머티리얼")]
    [SerializeField] private Material transparentMaterial;
    
    [Tooltip("기본 사용할 머티리얼 (Backface Culling)")]
    [SerializeField] private Material normalMaterial;
    
    // 자식 Plane들의 정보를 저장할 클래스
    [System.Serializable]
    private class PlaneInfo
    {
        public Renderer renderer;
        public Collider collider;
        public Material originalMaterial;
        public bool isDetectedByWallDetector;
    }
    
    // 자식 Plane 정보 배열
    private List<PlaneInfo> planeInfos = new List<PlaneInfo>();
    
    void Awake()
    {
        // 자식 오브젝트들의 Renderer와 Collider 찾기
        foreach (Transform child in transform)
        {
            Renderer childRenderer = child.GetComponent<Renderer>();
            Collider childCollider = child.GetComponent<Collider>();
            
            if (childRenderer != null && childCollider != null)
            {
                PlaneInfo info = new PlaneInfo
                {
                    renderer = childRenderer,
                    collider = childCollider,
                    originalMaterial = childRenderer.sharedMaterial,
                    isDetectedByWallDetector = false
                };
                
                planeInfos.Add(info);
                
                // Collider를 트리거로 설정
                childCollider.isTrigger = true;
                
                // 레이어 설정 (WallDetector가 감지할 수 있도록)
                child.gameObject.layer = LayerMask.NameToLayer("TransparentObstacle");
            }
        }
        
        Debug.Log($"SimplePlaneGroupController: {planeInfos.Count}개의 자식 Plane을 찾았습니다.");
    }
    
    void Update()
    {
        UpdateVisibility();
    }
    
    // WallDetector에 의한 감지 상태 설정 (외부에서 호출)
    public void SetDetectedByWallDetector(GameObject detectedObject, bool isDetected)
    {
        // 해당하는 Plane의 상태만 변경
        foreach (PlaneInfo info in planeInfos)
        {
            if (info.collider.gameObject == detectedObject)
            {
                info.isDetectedByWallDetector = isDetected;
                break;
            }
        }
    }
    
    // 시각화 업데이트
    private void UpdateVisibility()
    {
        foreach (PlaneInfo info in planeInfos)
        {
            if (info.renderer == null) continue;
            
            if (info.isDetectedByWallDetector && transparentMaterial != null)
            {
                // WallDetector에 감지되면 투명 머티리얼 적용
                info.renderer.material = transparentMaterial;
            }
            else
            {
                // 감지되지 않으면 기본 머티리얼 사용
                info.renderer.material = normalMaterial != null ? normalMaterial : info.originalMaterial;
            }
        }
    }
    
    // 컴포넌트가 비활성화될 때 원본 머티리얼 복원
    void OnDisable()
    {
        foreach (PlaneInfo info in planeInfos)
        {
            if (info.renderer != null && info.originalMaterial != null)
            {
                info.renderer.material = info.originalMaterial;
            }
        }
    }
    
    // 감지를 위해 모든 자식 Collider 반환
    public Collider[] GetAllPlaneColliders()
    {
        List<Collider> colliders = new List<Collider>();
        foreach (PlaneInfo info in planeInfos)
        {
            if (info.collider != null)
            {
                colliders.Add(info.collider);
            }
        }
        return colliders.ToArray();
    }
}