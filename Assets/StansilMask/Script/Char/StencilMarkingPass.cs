using UnityEngine;

public class StencilMarkingPass : MonoBehaviour
{
    [SerializeField] private Material stencilMaskMaterial;
    private Material cachedStencilMaterial;
    
    void Start()
    {
        // 스텐실 마스크 머티리얼 생성
        if (stencilMaskMaterial == null)
            stencilMaskMaterial = new Material(Shader.Find("Custom/StencilMarkOnly"));
        
        // 모든 하위 렌더러 찾기
        Renderer[] childRenderers = GetComponentsInChildren<Renderer>(true);
        
        if (childRenderers.Length == 0)
        {
            Debug.LogWarning("캐릭터에 렌더러가 없습니다.");
            return;
        }
        
        // 각 렌더러마다 스텐실 마스크 적용
        foreach (Renderer renderer in childRenderers)
        {
            CreateStencilMaskForRenderer(renderer);
        }
    }
    
    void CreateStencilMaskForRenderer(Renderer sourceRenderer)
    {
        // 렌더러 유형 확인
        if (sourceRenderer is SkinnedMeshRenderer)
        {
            SkinnedMeshRenderer smr = (SkinnedMeshRenderer)sourceRenderer;
            
            // 동일한 계층에 복제 오브젝트 생성
            GameObject maskObj = new GameObject(sourceRenderer.gameObject.name + "_StencilMask");
            maskObj.transform.SetParent(sourceRenderer.transform.parent);
            maskObj.transform.localPosition = sourceRenderer.transform.localPosition;
            maskObj.transform.localRotation = sourceRenderer.transform.localRotation;
            maskObj.transform.localScale = sourceRenderer.transform.localScale;
            
            // SkinnedMeshRenderer 복제 및 설정
            SkinnedMeshRenderer maskSMR = maskObj.AddComponent<SkinnedMeshRenderer>();
            maskSMR.sharedMesh = smr.sharedMesh;
            maskSMR.rootBone = smr.rootBone;
            maskSMR.bones = smr.bones;
            maskSMR.sharedMaterial = stencilMaskMaterial;
        }
        else if (sourceRenderer is MeshRenderer)
        {
            MeshFilter sourceMF = sourceRenderer.GetComponent<MeshFilter>();
            if (sourceMF == null || sourceMF.sharedMesh == null) return;
            
            // 동일한 계층에 복제 오브젝트 생성
            GameObject maskObj = new GameObject(sourceRenderer.gameObject.name + "_StencilMask");
            maskObj.transform.SetParent(sourceRenderer.transform.parent);
            maskObj.transform.localPosition = sourceRenderer.transform.localPosition;
            maskObj.transform.localRotation = sourceRenderer.transform.localRotation;
            maskObj.transform.localScale = sourceRenderer.transform.localScale;
            
            // MeshRenderer 및 MeshFilter 설정
            MeshFilter maskMF = maskObj.AddComponent<MeshFilter>();
            maskMF.sharedMesh = sourceMF.sharedMesh;
            
            MeshRenderer maskMR = maskObj.AddComponent<MeshRenderer>();
            maskMR.sharedMaterial = stencilMaskMaterial;
        }
    }
}