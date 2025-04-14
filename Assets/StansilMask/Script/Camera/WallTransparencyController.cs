using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

// 이코드는 사용하지 않는 코드이며 예제를 위해 남겨둠.
public class WallTransparencyController : MonoBehaviour
{
    [Header("기본 설정")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform cameraTrans;
    [SerializeField] private LayerMask wallLayers;
    [SerializeField] private Material transparentMaterial;
    
    [Header("투명 영역 설정")]
    [SerializeField] private GameObject transparentZonePrefab;
    [SerializeField] private float zoneDistanceOffset = 2f;
    [Range(0.2f, 1.0f)]
    [SerializeField] private float zoneHeightFactor = 0.25f;
    [Range(1.0f, 1.5f)]
    [SerializeField] private float zoneWidthFactor = 1.2f;
    
    [Header("디버그 설정")]
    [SerializeField] private bool visualizeZone = false;
    [SerializeField] private Color zoneColor = new Color(1f, 0f, 0f, 0.5f);

    // 내부 변수
    private GameObject transparentZone;
    private Material debugMaterial, standardMaterial;
    private LineRenderer debugLineRenderer, cubeOutlineRenderer;
    private List<Renderer> wallRenderers = new List<Renderer>();
    private Dictionary<Renderer, Material[]> originalMaterialsMap = new Dictionary<Renderer, Material[]>();

    void Start()
    {
        InitializeTransparentZone();
        CreateDebugVisualization();
        UpdateVisualization();
    }
    
    void Update()
    {
        UpdateTransparentZone();
        DetectWallsBetweenCameraAndPlayer();
    }
    
    #region 초기화 및 시각화

    void InitializeTransparentZone()
    {
        transparentZone = transparentZonePrefab ? Instantiate(transparentZonePrefab)
                                                : CreateTransparentZone();
        transparentZone.transform.SetParent(cameraTrans, false);
        transparentZone.transform.localPosition = Vector3.zero;
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
    
    GameObject CreateTransparentZone()
    {
        GameObject zone = new GameObject("TransparentZone");
        MeshFilter mf = zone.AddComponent<MeshFilter>();
        MeshRenderer mr = zone.AddComponent<MeshRenderer>();
        GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mf.sharedMesh = tempCube.GetComponent<MeshFilter>().sharedMesh;
        Destroy(tempCube);
        return zone;
    }
    
    void CreateDebugVisualization()
    {
        debugLineRenderer = CreateLineRenderer("DebugLine", transform, Color.yellow, 2);
        cubeOutlineRenderer = CreateLineRenderer("CubeOutline", transparentZone.transform, Color.green, 24);
        UpdateVisualization();
    }

    LineRenderer CreateLineRenderer(string name, Transform parent, Color col, int posCount)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.startWidth = lr.endWidth = 0.05f;
        lr.positionCount = posCount;
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = col;
        lr.material = mat;
        lr.enabled = visualizeZone;
        return lr;
    }
    
    void UpdateVisualization()
    {
        if (transparentZone && transparentZone.TryGetComponent<MeshRenderer>(out MeshRenderer mr))
        {
            mr.material = visualizeZone ? debugMaterial : standardMaterial;
            mr.enabled = true;
        }
        if (debugLineRenderer != null) debugLineRenderer.enabled = visualizeZone;
        if (cubeOutlineRenderer != null) cubeOutlineRenderer.enabled = visualizeZone;
    }
    
    #endregion
    
    #region 투명 영역 업데이트

    void UpdateTransparentZone()
    {
        if (!transparentZone || !player || !cameraTrans) return;

        Camera cam = cameraTrans.GetComponent<Camera>();
        if (cam == null) return;

        float distance = (player.position - cameraTrans.position).magnitude;
        float cameraToEdgeDistance = CalculateFloorEdgeDistance(out bool foundFloor);
        float width = CalculateCameraWidthAtDistance(cam, distance);
        transparentZone.transform.localPosition = new Vector3(0, 0, cameraToEdgeDistance * 0.5f);
        transparentZone.transform.localRotation = Quaternion.identity;

        float actualHeight = CalculateRequiredZoneHeight();
        float zoneHeight = (actualHeight > 0 ? actualHeight : distance) * zoneHeightFactor;

        Vector3 scale = new Vector3(width * zoneWidthFactor, zoneHeight, cameraToEdgeDistance - zoneDistanceOffset);
        transparentZone.transform.localScale = scale;
        if (visualizeZone)
            UpdateCubeOutline();
    }

    float CalculateRequiredZoneHeight()
    {
        return Physics.Raycast(player.position, Vector3.up, out RaycastHit hit, 10f, wallLayers)
            ? hit.distance + 1.0f
            : Vector3.Distance(cameraTrans.position, player.position) * 2f;
    }
    
    float CalculateFloorEdgeDistance(out bool foundFloor)
    {
        foundFloor = false;
        float camEdgeDist = 5f;
        float minDistance = float.MaxValue;
        RaycastHit floorHit;
        Vector3[] detectionDirections = { Vector3.down, -cameraTrans.up, -player.up, (Vector3.down - cameraTrans.up).normalized };

        foreach (Vector3 dir in detectionDirections)
        {
            if (Physics.Raycast(player.position, dir, out floorHit, 5f, wallLayers))
            {
                if (floorHit.collider == null) continue;
                foundFloor = true;
                float d = floorHit.collider is BoxCollider box ?
                            Vector3.Distance(cameraTrans.position, FindClosestEdgePointToCamera(box)) :
                            Vector3.Distance(cameraTrans.position, floorHit.collider.bounds.ClosestPoint(cameraTrans.position));
                if (d < minDistance) minDistance = d;
            }
        }
        return foundFloor ? minDistance : camEdgeDist;
    }

    Vector3 FindClosestEdgePointToCamera(BoxCollider box)
    {
        Vector3 ext = box.size / 2f;
        Transform t = box.transform;
        Vector3 closest = Vector3.zero;
        float minDist = float.MaxValue;
        // 8 코너 순회
        for (int x = -1; x <= 1; x += 2)
            for (int y = -1; y <= 1; y += 2)
                for (int z = -1; z <= 1; z += 2)
                {
                    Vector3 worldCorner = t.TransformPoint(box.center + new Vector3(ext.x * x, ext.y * y, ext.z * z));
                    float d = Vector3.Distance(cameraTrans.position, worldCorner);
                    if (d < minDist) { minDist = d; closest = worldCorner; }
                }
        return closest;
    }
    
    float CalculateCameraWidthAtDistance(Camera cam, float distance)
    {
        if (cam.orthographic)
            return cam.orthographicSize * 2f * cam.aspect;
        float halfFov = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;
        return 2f * distance * Mathf.Tan(halfFov) * cam.aspect;
    }
    
    void UpdateCubeOutline()
    {
        if (cubeOutlineRenderer == null) return;
        Vector3[] verts = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f, -0.5f,  0.5f),
            new Vector3(-0.5f, -0.5f,  0.5f),
            new Vector3(-0.5f,  0.5f, -0.5f),
            new Vector3( 0.5f,  0.5f, -0.5f),
            new Vector3( 0.5f,  0.5f,  0.5f),
            new Vector3(-0.5f,  0.5f,  0.5f)
        };

        int idx = 0;
        // 밑면
        for (int i = 0; i < 4; i++)
        {
            int next = (i + 1) % 4;
            cubeOutlineRenderer.SetPosition(idx++, transparentZone.transform.TransformPoint(verts[i]));
            cubeOutlineRenderer.SetPosition(idx++, transparentZone.transform.TransformPoint(verts[next]));
        }
        // 윗면
        for (int i = 4; i < 8; i++)
        {
            int next = ((i - 4 + 1) % 4) + 4;
            cubeOutlineRenderer.SetPosition(idx++, transparentZone.transform.TransformPoint(verts[i]));
            cubeOutlineRenderer.SetPosition(idx++, transparentZone.transform.TransformPoint(verts[next]));
        }
        // 측면 연결
        for (int i = 0; i < 4; i++)
        {
            cubeOutlineRenderer.SetPosition(idx++, transparentZone.transform.TransformPoint(verts[i]));
            cubeOutlineRenderer.SetPosition(idx++, transparentZone.transform.TransformPoint(verts[i + 4]));
        }
    }

    #endregion
    
    #region 벽 투명화 처리

    void DetectWallsBetweenCameraAndPlayer()
    {
        if (!player || !cameraTrans) return;
        RestoreOriginalMaterials();
        List<GameObject> excluded = GetExcludedFloorObjects();

        Collider[] cols = Physics.OverlapBox(transparentZone.transform.position,
                                             transparentZone.transform.lossyScale / 2f,
                                             transparentZone.transform.rotation,
                                             wallLayers);
        ApplyTransparentMaterial(cols, excluded);
    }
    
    List<GameObject> GetExcludedFloorObjects()
    {
        List<GameObject> list = new List<GameObject>();
        foreach (Collider col in Physics.OverlapSphere(player.position, 1f, wallLayers))
        {
            if (col.bounds.max.y <= player.position.y + 0.1f)
                list.Add(col.gameObject);
        }
        return list;
    }
    
    void ApplyTransparentMaterial(Collider[] colliders, List<GameObject> excluded)
    {
        foreach (Collider col in colliders)
        {
            if (excluded.Contains(col.gameObject)) continue;
            Renderer rend = col.GetComponent<Renderer>();
            if (rend == null) continue;
            
            if (!originalMaterialsMap.ContainsKey(rend))
                originalMaterialsMap[rend] = rend.materials;
            
            Material[] newMats = new Material[rend.materials.Length];
            for (int i = 0; i < newMats.Length; i++)
                newMats[i] = transparentMaterial;
            
            rend.materials = newMats;
            wallRenderers.Add(rend);
        }
    }
    
    void RestoreOriginalMaterials()
    {
        foreach (Renderer rend in wallRenderers)
        {
            if (rend && originalMaterialsMap.TryGetValue(rend, out Material[] mats))
                rend.materials = mats;
        }
        wallRenderers.Clear();
    }

    #endregion
    
    void OnDisable()
    {
        RestoreOriginalMaterials();
    }
}
