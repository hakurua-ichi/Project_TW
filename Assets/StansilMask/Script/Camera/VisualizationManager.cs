using UnityEngine;

/// <summary>
/// 디버그 시각화 관리
/// </summary>
public class VisualizationManager
{
    private bool visualizeZone;
    private Color zoneColor;
    private Material debugMaterial;
    private Material standardMaterial;
    private LineRenderer debugLineRenderer;
    private LineRenderer cubeOutlineRenderer;
    private GameObject transparentZone; // 투명 영역 참조 추가

    public VisualizationManager(bool visualizeZone, Color zoneColor)
    {
        this.visualizeZone = visualizeZone;
        this.zoneColor = zoneColor;

        InitializeMaterials();
    }

    private void InitializeMaterials()
    {
        standardMaterial = new Material(Shader.Find("Custom/TransparentZone"));

        debugMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        debugMaterial.color = zoneColor;
        debugMaterial.SetFloat("_Surface", 1);
        debugMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        debugMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        debugMaterial.SetInt("_ZWrite", 0);
        debugMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        debugMaterial.renderQueue = 3000;
    }

    public void SetupVisualization(GameObject zone)
    {
        if (!zone) return;
        this.transparentZone = zone; // 투명 영역 참조 저장

        // 라인 렌더러 생성 및 설정
        GameObject lineObj = new GameObject("DebugLine");
        lineObj.transform.SetParent(null); // 월드 좌표계에 고정
        debugLineRenderer = lineObj.AddComponent<LineRenderer>();
        debugLineRenderer.startWidth = 0.05f;
        debugLineRenderer.endWidth = 0.05f;
        debugLineRenderer.positionCount = 2;
        debugLineRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = Color.yellow };
        debugLineRenderer.enabled = visualizeZone;

        // 큐브 외곽선 생성 및 설정
        GameObject outlineObj = new GameObject("CubeOutline");
        outlineObj.transform.SetParent(zone.transform, false);
        cubeOutlineRenderer = outlineObj.AddComponent<LineRenderer>();
        cubeOutlineRenderer.startWidth = 0.05f;
        cubeOutlineRenderer.endWidth = 0.05f;
        cubeOutlineRenderer.positionCount = 24; // 12 모서리 = 24 점
        cubeOutlineRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = Color.green };
        cubeOutlineRenderer.enabled = visualizeZone;
        
    }

    public void UpdateVisualization(bool visualize)
    {
        if (visualizeZone == visualize) return; // 변경이 없으면 스킵
        
        visualizeZone = visualize;

        if (debugLineRenderer != null) debugLineRenderer.enabled = visualizeZone;
        if (cubeOutlineRenderer != null) cubeOutlineRenderer.enabled = visualizeZone;
        
    }

    public void UpdateVisualElements(Vector3 cameraPosition, Vector3 playerPosition)
    {
        if (!visualizeZone) return;

        // 카메라-플레이어 연결선 업데이트
        if (debugLineRenderer != null)
        {
            debugLineRenderer.SetPosition(0, cameraPosition);
            debugLineRenderer.SetPosition(1, playerPosition);
        }
        
        // 투명 영역 외곽선 업데이트 - 누락된 부분 추가
        UpdateCubeOutline();
    }
    
    // 큐브 외곽선 업데이트 함수 추가
    private void UpdateCubeOutline()
    {
        if (cubeOutlineRenderer == null || transparentZone == null) 
        {
            Debug.LogWarning($"큐브 외곽선 업데이트 실패: renderer={cubeOutlineRenderer!=null}, zone={transparentZone!=null}");
            return;
        }
        
        // 큐브 정점 정의 (로컬 좌표)
        Vector3[] vertices = new Vector3[8];
        vertices[0] = new Vector3(-0.5f, -0.5f, -0.5f);
        vertices[1] = new Vector3(0.5f, -0.5f, -0.5f);
        vertices[2] = new Vector3(0.5f, -0.5f, 0.5f);
        vertices[3] = new Vector3(-0.5f, -0.5f, 0.5f);
        vertices[4] = new Vector3(-0.5f, 0.5f, -0.5f);
        vertices[5] = new Vector3(0.5f, 0.5f, -0.5f);
        vertices[6] = new Vector3(0.5f, 0.5f, 0.5f);
        vertices[7] = new Vector3(-0.5f, 0.5f, 0.5f);
        
        int index = 0;
        
        // 아래쪽 면 (4개 선)
        for (int i = 0; i < 4; i++)
        {
            int next = (i + 1) % 4;
            cubeOutlineRenderer.SetPosition(index++, transparentZone.transform.TransformPoint(vertices[i]));
            cubeOutlineRenderer.SetPosition(index++, transparentZone.transform.TransformPoint(vertices[next]));
        }
        
        // 위쪽 면 (4개 선)
        for (int i = 4; i < 8; i++)
        {
            int next = 4 + ((i - 4 + 1) % 4);
            cubeOutlineRenderer.SetPosition(index++, transparentZone.transform.TransformPoint(vertices[i]));
            cubeOutlineRenderer.SetPosition(index++, transparentZone.transform.TransformPoint(vertices[next]));
        }
        
        // 측면 연결 (4개 수직선)
        for (int i = 0; i < 4; i++)
        {
            cubeOutlineRenderer.SetPosition(index++, transparentZone.transform.TransformPoint(vertices[i]));
            cubeOutlineRenderer.SetPosition(index++, transparentZone.transform.TransformPoint(vertices[i + 4]));
        }
        
    }

    /// <summary>
    /// 시각화 영역 색상 업데이트
    /// </summary>
    public void UpdateZoneColor(Color newColor)
    {
        this.zoneColor = newColor;
        
        // 디버그 머티리얼 색상 업데이트
        if (debugMaterial != null)
        {
            debugMaterial.color = zoneColor;
        }
    }
}