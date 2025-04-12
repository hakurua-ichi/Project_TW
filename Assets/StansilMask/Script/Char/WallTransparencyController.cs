using UnityEngine;
using System.Collections.Generic;

public class WallTransparencyController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform cameraTrans;
    [SerializeField] private LayerMask wallLayers;
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private float viewConeAngle = 60f;
    
    private Material[] originalMaterials;
    private Renderer[] wallRenderers = new Renderer[0];
    private Dictionary<Renderer, Material[]> originalMaterialsMap = new Dictionary<Renderer, Material[]>();
    
    [SerializeField] private GameObject transparentZonePrefab;
    private GameObject transparentZone;
    
    void Start()
    {
        if (transparentZonePrefab == null)
        {
            // 투명 영역 프리팹 생성 로직
            transparentZone = CreateTransparentZone();
        }
        else
        {
            transparentZone = Instantiate(transparentZonePrefab);
        }
    }
    
    GameObject CreateTransparentZone()
    {
        GameObject zone = new GameObject("TransparentZone");
        MeshFilter mf = zone.AddComponent<MeshFilter>();
        MeshRenderer mr = zone.AddComponent<MeshRenderer>();
        
        // 콘 형태의 메시 생성 (카메라->플레이어 방향으로 확장)
        // (메시 생성 코드는 복잡하므로 간략화)
        
        // 투명 영역 셰이더 적용
        mr.material = new Material(Shader.Find("Custom/TransparentZone"));
        
        return zone;
    }
    
    void Update()
    {
        // 투명 영역 위치 및 크기 업데이트
        UpdateTransparentZone();
        
        // 카메라와 플레이어 사이의 오브젝트 탐지
        DetectWallsBetweenCameraAndPlayer();
    }
    
    void UpdateTransparentZone()
    {
        if (!transparentZone || !player || !cameraTrans) return;
        
        // 카메라에서 플레이어 방향으로 투명 영역 조정
        Vector3 direction = player.position - cameraTrans.position;
        float distance = direction.magnitude;
        
        transparentZone.transform.position = cameraTrans.position;
        transparentZone.transform.rotation = Quaternion.LookRotation(direction);
        transparentZone.transform.localScale = new Vector3(distance * 0.2f, distance * 0.2f, distance);
    }
    
    void DetectWallsBetweenCameraAndPlayer()
    {
        if (!player || !cameraTrans) return;
        
        // 이전에 변경된 재질 복원
        RestoreOriginalMaterials();
        
        // 카메라와 플레이어 사이의 벽 감지
        Vector3 direction = player.position - cameraTrans.position;
        float distance = direction.magnitude;
        
        RaycastHit[] hits = Physics.SphereCastAll(
            cameraTrans.position, 
            0.5f, // 반경 
            direction.normalized, 
            distance, 
            wallLayers
        );
        
        // 감지된 벽에 투명 재질 적용
        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend)
            {
                if (!originalMaterialsMap.ContainsKey(rend))
                {
                    originalMaterialsMap[rend] = rend.materials;
                }
                
                Material[] newMaterials = new Material[rend.materials.Length];
                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = transparentMaterial;
                }
                rend.materials = newMaterials;
                
                // 현재 감지된 벽 렌더러 저장
                System.Array.Resize(ref wallRenderers, wallRenderers.Length + 1);
                wallRenderers[wallRenderers.Length - 1] = rend;
            }
        }
    }
    
    void RestoreOriginalMaterials()
    {
        foreach (Renderer rend in wallRenderers)
        {
            if (rend && originalMaterialsMap.ContainsKey(rend))
            {
                rend.materials = originalMaterialsMap[rend];
            }
        }
        wallRenderers = new Renderer[0];
    }
    
    void OnDisable()
    {
        RestoreOriginalMaterials();
    }
}