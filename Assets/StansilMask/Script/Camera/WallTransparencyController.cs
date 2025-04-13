using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class WallTransparencyController : MonoBehaviour
{
    [Header("기본 설정")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform cameraTrans;
    [SerializeField] private LayerMask wallLayers;
    [SerializeField] private Material transparentMaterial;
    
    [Header("투명 영역 설정")]
    [SerializeField] private GameObject transparentZonePrefab;
    [Range(0.2f, 1.0f)]
    [SerializeField] private float zoneHeightFactor = 0.25f;
    [Range(1.0f, 1.5f)]
    [SerializeField] private float zoneWidthFactor = 1.2f;
    
    [Header("디버그 설정")]
    [Tooltip("체크하면 투명 영역이 시각화됩니다")]
    [SerializeField] private bool visualizeZone = false; 
    [SerializeField] private Color zoneColor = new Color(1.0f, 0.0f, 0.0f, 0.5f);
    [SerializeField] private Key visualizeKey = Key.V;

    // 내부 사용 변수
    private GameObject transparentZone;
    private Material debugMaterial;
    private Material standardMaterial;
    private bool previousVisualizeState;
    private LineRenderer debugLineRenderer;
    private LineRenderer cubeOutlineRenderer;
    private Renderer[] wallRenderers = new Renderer[0];
    private Dictionary<Renderer, Material[]> originalMaterialsMap = new Dictionary<Renderer, Material[]>();

    void Start()
    {
        InitializeTransparentZone();
        CreateDebugVisualization();
        
        previousVisualizeState = visualizeZone;
        UpdateVisualization();
        
        Debug.Log("WallTransparencyController 초기화 완료");
    }
    
    void Update()
    {
        // 디버그 시각화 상태 변경 감지
        if (previousVisualizeState != visualizeZone || 
            (Keyboard.current != null && Keyboard.current[visualizeKey].wasPressedThisFrame))
        {
            if (Keyboard.current != null && Keyboard.current[visualizeKey].wasPressedThisFrame)
            {
                visualizeZone = !visualizeZone;
            }
            previousVisualizeState = visualizeZone;
            UpdateVisualization();
        }
        
        // 투명 영역 위치 및 크기 업데이트
        UpdateTransparentZone();
        
        // 카메라와 플레이어 사이의 오브젝트 탐지
        DetectWallsBetweenCameraAndPlayer();
    }
    
    #region 초기화 및 설정
    
    private void InitializeTransparentZone()
    {
        // 투명 Zone 초기화
        if (transparentZonePrefab == null)
        {
            transparentZone = CreateTransparentZone();
        }
        else
        {
            transparentZone = Instantiate(transparentZonePrefab);
        }
        
        // Zone을 카메라의 자식으로 설정
        transparentZone.transform.parent = cameraTrans;
        transparentZone.transform.localPosition = Vector3.zero;
        
        // 머티리얼 초기화
        standardMaterial = new Material(Shader.Find("Custom/TransparentZone"));
        
        // 디버깅용 머티리얼 설정
        debugMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        debugMaterial.color = zoneColor;
        debugMaterial.SetFloat("_Surface", 1);
        debugMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        debugMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        debugMaterial.SetInt("_ZWrite", 0);
        debugMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        debugMaterial.renderQueue = 3000;
    }
    
    private void CreateDebugVisualization()
    {
        // 디버그 라인 생성
        GameObject lineObj = new GameObject("DebugLine");
        lineObj.transform.parent = transform;
        debugLineRenderer = lineObj.AddComponent<LineRenderer>();
        debugLineRenderer.startWidth = 0.05f;
        debugLineRenderer.endWidth = 0.05f;
        debugLineRenderer.positionCount = 2;
        debugLineRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        debugLineRenderer.material.color = Color.yellow;
        debugLineRenderer.enabled = visualizeZone;
        
        // 큐브 외곽선 생성
        GameObject outlineObj = new GameObject("CubeOutline");
        outlineObj.transform.parent = transparentZone.transform;
        outlineObj.transform.localPosition = Vector3.zero;
        
        cubeOutlineRenderer = outlineObj.AddComponent<LineRenderer>();
        cubeOutlineRenderer.startWidth = 0.05f;
        cubeOutlineRenderer.endWidth = 0.05f;
        cubeOutlineRenderer.positionCount = 24; // 12개 모서리 * 2개 점
        cubeOutlineRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        cubeOutlineRenderer.material.color = Color.green;
        cubeOutlineRenderer.enabled = visualizeZone;
    }
    
    private GameObject CreateTransparentZone()
    {
        GameObject zone = new GameObject("TransparentZone");
        MeshFilter mf = zone.AddComponent<MeshFilter>();
        MeshRenderer mr = zone.AddComponent<MeshRenderer>();
        
        // 간단한 큐브 메시 사용
        GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mf.sharedMesh = tempCube.GetComponent<MeshFilter>().sharedMesh;
        Destroy(tempCube);
        
        return zone;
    }
    
    private void UpdateVisualization()
    {
        if (transparentZone != null && transparentZone.TryGetComponent<MeshRenderer>(out var renderer))
        {
            renderer.material = visualizeZone ? debugMaterial : standardMaterial;
            renderer.enabled = true;
        }
        
        if (debugLineRenderer != null) debugLineRenderer.enabled = visualizeZone;
        if (cubeOutlineRenderer != null) cubeOutlineRenderer.enabled = visualizeZone;
    }
    
    #endregion
    
    #region 투명 영역 업데이트
    
    private void UpdateTransparentZone()
    {
        if (!transparentZone || !player || !cameraTrans) return;
        
        Camera cam = cameraTrans.GetComponent<Camera>();
        if (cam == null) return;

        // 카메라에서 플레이어까지의 방향과 거리
        Vector3 direction = player.position - cameraTrans.position;
        float distance = direction.magnitude;
        
        // 발판의 카메라 방향 끝점에서 카메라까지의 거리 계산
        float cameraToEdgeDistance = CalculateFloorEdgeDistance(out bool foundFloor);

        // FOV 기반 너비 계산
        float width = CalculateCameraWidthAtDistance(cam, distance);
        
        // 투명 영역 위치 설정 
        transparentZone.transform.localPosition = new Vector3(0, 0, cameraToEdgeDistance * 0.5f);
        transparentZone.transform.localRotation = Quaternion.identity;
        
        // 높이 계산 개선 - 맵 회전과 무관하게 일정한 높이 유지
        float zoneHeight;
        
        // 플레이어 기준으로 위아래 레이캐스트하여 실제 필요한 높이 계산 (회전 맵 대응)
        float actualHeight = CalculateRequiredZoneHeight();
        if (actualHeight > 0)
        {
            // 실제로 감지된 높이가 있으면 사용
            zoneHeight = actualHeight * zoneHeightFactor;
        }
        else
        {
            // 감지 안되면 기존 방식 유지 
            zoneHeight = distance * zoneHeightFactor;
        }
        
        // 투명 영역 크기 설정
        Vector3 scale = new Vector3(
            width * zoneWidthFactor,     // 너비는 FOV 기반으로 확장
            zoneHeight,                  // 개선된 높이 계산
            cameraToEdgeDistance         // 카메라-발판 끝 거리
        );
        transparentZone.transform.localScale = scale;
        
        // 디버그 시각화 업데이트
        UpdateDebugVisualization(foundFloor);
    }

    private float CalculateRequiredZoneHeight()
    {
        float height = 0;
        RaycastHit hit;
        
        // 플레이어 위/아래로 레이캐스트하여 통로 높이 감지
        if (Physics.Raycast(player.position, Vector3.up, out hit, 10f, wallLayers))
        {
            // 플레이어 위 천장까지의 거리
            float ceilingHeight = hit.distance;
            
            // 플레이어가 서있는 바닥부터 천장까지의 총 높이 계산
            height = ceilingHeight + 1.0f; // 플레이어 키 감안 추가
        }
        else
        {
            // 천장이 감지되지 않으면 기본값 (카메라-플레이어 거리의 2배)
            height = Vector3.Distance(cameraTrans.position, player.position) * 2.0f;
        }
        
        return height;
    }

    private float CalculateFloorEdgeDistance(out bool foundFloor)
    {
        foundFloor = false;
        float cameraToEdgeDistance = 5f; // 기본값
        
        if (!player || !cameraTrans) return cameraToEdgeDistance;
        
        // 개선: 다양한 방향에서 바닥 감지 시도
        Vector3[] detectionDirections = {
            Vector3.down,
            -cameraTrans.up,
            -player.up,
            (Vector3.down - cameraTrans.up).normalized
        };
        
        RaycastHit floorHit;
        Vector3 closestEdgePoint = Vector3.zero;
        float minDistance = float.MaxValue;
        
        // 플레이어 발 아래 바닥 감지 시도
        foreach (Vector3 dir in detectionDirections)
        {
            if (Physics.Raycast(player.position, dir.normalized, out floorHit, 5f, wallLayers))
            {
                Collider floorCollider = floorHit.collider;
                if (floorCollider == null) continue;
                
                foundFloor = true;
                
                // BoxCollider인 경우 OBB 방식 사용
                BoxCollider boxCollider = floorCollider as BoxCollider;
                if (boxCollider != null)
                {
                    // 카메라에서 가장 가까운 발판 모서리 찾기 (OBB)
                    Vector3 edgePoint = FindClosestEdgePointToCamera(boxCollider);
                    
                    // 카메라에서 모서리까지의 거리 계산
                    float distToCamera = Vector3.Distance(cameraTrans.position, edgePoint);
                    
                    if (distToCamera < minDistance)
                    {
                        minDistance = distToCamera;
                        closestEdgePoint = edgePoint;
                    }
                }
                else
                {
                    // BoxCollider가 아닌 경우 AABB 방식 사용
                    Vector3 edgePoint = FindClosestAABBEdgePointToCamera(floorCollider.bounds);
                    
                    float distToCamera = Vector3.Distance(cameraTrans.position, edgePoint);
                    if (distToCamera < minDistance)
                    {
                        minDistance = distToCamera;
                        closestEdgePoint = edgePoint;
                    }
                }
            }
        }
        
        if (foundFloor && minDistance < float.MaxValue)
        {
            cameraToEdgeDistance = minDistance;
            
            // 디버그 시각화 - 카메라와 가장 가까운 발판 모서리 연결
            if (visualizeZone)
            {
                Debug.DrawLine(cameraTrans.position, closestEdgePoint, Color.cyan, 0.1f);
                Debug.Log($"카메라에서 발판 끝까지 거리: {cameraToEdgeDistance}");
            }
        }
        
        return cameraToEdgeDistance;
    }

    private Vector3 FindClosestEdgePointToCamera(BoxCollider boxCollider)
    {
        Vector3 extents = boxCollider.size / 2;
        Transform floorTransform = boxCollider.transform;
        Vector3 closestPoint = Vector3.zero;
        float minDistance = float.MaxValue;
        
        // 바닥의 모든 모서리 점들 순회 (상단 모서리 포함)
        for (int x = -1; x <= 1; x += 2)
        {
            for (int y = -1; y <= 1; y += 2)
            {
                for (int z = -1; z <= 1; z += 2)
                {
                    // 로컬 좌표의 코너 위치
                    Vector3 corner = new Vector3(
                        boxCollider.center.x + extents.x * x,
                        boxCollider.center.y + extents.y * y,
                        boxCollider.center.z + extents.z * z
                    );
                    
                    // 로컬 -> 월드 변환
                    Vector3 worldCorner = floorTransform.TransformPoint(corner);
                    
                    // 카메라까지의 거리 계산
                    float distToCamera = Vector3.Distance(cameraTrans.position, worldCorner);
                    
                    // 최단 거리 갱신
                    if (distToCamera < minDistance)
                    {
                        minDistance = distToCamera;
                        closestPoint = worldCorner;
                    }
                }
            }
        }
        
        // 추가: 모서리 외에도 경계면에서 더 가까운 지점이 있을 수 있음
        // 6개 면의 중심점 체크 (더 정확한 결과를 위해)
        for (int axis = 0; axis < 3; axis++)  // X, Y, Z 축
        {
            for (int dir = -1; dir <= 1; dir += 2)  // 각 축의 양/음 방향
            {
                Vector3 faceCenter = Vector3.zero;
                
                // 면의 중심 계산 (로컬 좌표)
                if (axis == 0)  // X축 면
                    faceCenter = new Vector3(boxCollider.center.x + extents.x * dir, boxCollider.center.y, boxCollider.center.z);
                else if (axis == 1)  // Y축 면
                    faceCenter = new Vector3(boxCollider.center.x, boxCollider.center.y + extents.y * dir, boxCollider.center.z);
                else  // Z축 면
                    faceCenter = new Vector3(boxCollider.center.x, boxCollider.center.y, boxCollider.center.z + extents.z * dir);
                
                // 로컬 -> 월드 변환
                Vector3 worldFaceCenter = floorTransform.TransformPoint(faceCenter);
                
                // 카메라 방향 벡터
                Vector3 cameraToCenterDir = (worldFaceCenter - cameraTrans.position).normalized;
                
                // 면의 법선 벡터 (월드 공간)
                Vector3 faceNormal = Vector3.zero;
                if (axis == 0)
                    faceNormal = floorTransform.TransformDirection(Vector3.right) * dir;
                else if (axis == 1)
                    faceNormal = floorTransform.TransformDirection(Vector3.up) * dir;
                else
                    faceNormal = floorTransform.TransformDirection(Vector3.forward) * dir;
                
                // 카메라가 면을 바라보는 경우만 고려 (내적 < 0)
                float alignment = Vector3.Dot(faceNormal, cameraToCenterDir);
                if (alignment < 0)
                {
                    // 면에 카메라에서 가장 가까운 지점 계산
                    Vector3 closestOnFace = Physics.ClosestPoint(cameraTrans.position, boxCollider, worldFaceCenter, Quaternion.identity);
                    float distToCamera = Vector3.Distance(cameraTrans.position, closestOnFace);
                    
                    if (distToCamera < minDistance)
                    {
                        minDistance = distToCamera;
                        closestPoint = closestOnFace;
                    }
                }
            }
        }
        
        return closestPoint;
    }

    private Vector3 FindClosestAABBEdgePointToCamera(Bounds bounds)
    {
        Vector3 closestPoint = bounds.ClosestPoint(cameraTrans.position);
        
        // AABB에서는 ClosestPoint가 이미 효율적으로 가장 가까운 점을 찾아줌
        return closestPoint;
    }

    private float CalculateCameraWidthAtDistance(Camera cam, float distance)
    {
        if (cam.orthographic)
        {
            return cam.orthographicSize * 2.0f * cam.aspect;
        }
        else
        {
            float halfFov = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;
            float height = 2.0f * distance * Mathf.Tan(halfFov);
            return height * cam.aspect;
        }
    }
    
    private void UpdateDebugVisualization(bool foundFloor)
    {
        if (!visualizeZone) return;
        
        // 카메라-플레이어 라인 시각화
        if (debugLineRenderer != null && debugLineRenderer.enabled)
        {
            debugLineRenderer.SetPosition(0, cameraTrans.position);
            debugLineRenderer.SetPosition(1, player.position);
        }
        
        // 바닥 감지 방향 시각화
        Debug.DrawLine(player.position, player.position + Vector3.down * 5f, Color.magenta, 0.1f);
        
        // 투명 영역 외곽선 업데이트
        UpdateCubeOutline();
    }
    
    private void UpdateCubeOutline()
    {
        if (cubeOutlineRenderer == null) return;
        
        // 유니티 큐브는 -0.5 ~ 0.5 범위의 로컬 좌표를 가짐
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
        for (int i = 0; i < 4; i++)
        {
            int next = (i + 1) % 4;
            cubeOutlineRenderer.SetPosition(index++, transparentZone.transform.TransformPoint(vertices[i+4]));
            cubeOutlineRenderer.SetPosition(index++, transparentZone.transform.TransformPoint(vertices[next+4]));
        }
        
        // 측면 연결 (4개 선)
        for (int i = 0; i < 4; i++)
        {
            cubeOutlineRenderer.SetPosition(index++, transparentZone.transform.TransformPoint(vertices[i]));
            cubeOutlineRenderer.SetPosition(index++, transparentZone.transform.TransformPoint(vertices[i+4]));
        }
    }
    
    #endregion
    
    #region 벽 투명화 처리
    
    private void DetectWallsBetweenCameraAndPlayer()
    {
        if (!player || !cameraTrans) return;
        
        // 이전에 변경된 재질 복원
        RestoreOriginalMaterials();
        
        // 플레이어가 밟고 있는 바닥은 제외 처리
        List<GameObject> excludedObjects = GetExcludedFloorObjects();
        
        // Zone의 월드 위치/회전/크기 가져오기
        Vector3 zonePosition = transparentZone.transform.position;
        Vector3 zoneSize = transparentZone.transform.lossyScale;
        Quaternion zoneRotation = transparentZone.transform.rotation;
        
        // OverlapBox로 Zone 내부의 벽 감지
        Collider[] colliders = Physics.OverlapBox(
            zonePosition,
            zoneSize / 2,
            zoneRotation,
            wallLayers
        );
        
        // 감지된 벽에 투명 재질 적용
        ApplyTransparentMaterial(colliders, excludedObjects);
    }
    
    private List<GameObject> GetExcludedFloorObjects()
    {
        List<GameObject> excludedObjects = new List<GameObject>();
        
        // 플레이어 근처의 바닥 오브젝트 검색
        Collider[] nearbyColliders = Physics.OverlapSphere(player.position, 1.0f, wallLayers);
        foreach (Collider col in nearbyColliders)
        {
            // 플레이어 근처의 바닥은 모두 제외 (플레이어 위치보다 약간 위까지)
            if (col.bounds.max.y <= player.position.y + 0.1f)
            {
                excludedObjects.Add(col.gameObject);
                if (visualizeZone) Debug.Log($"제외된 바닥: {col.gameObject.name}");
            }
        }
        
        return excludedObjects;
    }
    
    private void ApplyTransparentMaterial(Collider[] colliders, List<GameObject> excludedObjects)
    {
        foreach (Collider col in colliders)
        {
            // 제외 목록에 있는 오브젝트는 건너뛰기
            if (excludedObjects.Contains(col.gameObject))
            {
                continue;
            }
            
            // 렌더러에 투명 재질 적용
            Renderer rend = col.GetComponent<Renderer>();
            if (rend)
            {
                if (visualizeZone) Debug.Log($"투명화 적용: {col.gameObject.name}");
                
                // 원본 재질 저장
                if (!originalMaterialsMap.ContainsKey(rend))
                {
                    originalMaterialsMap[rend] = rend.materials;
                }
                
                // 투명 재질로 교체
                Material[] newMaterials = new Material[rend.materials.Length];
                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = transparentMaterial;
                }
                rend.materials = newMaterials;
                
                // 현재 투명화된 렌더러 저장
                System.Array.Resize(ref wallRenderers, wallRenderers.Length + 1);
                wallRenderers[wallRenderers.Length - 1] = rend;
            }
        }
    }
    
    private void RestoreOriginalMaterials()
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
    
    #endregion
    
    void OnDisable()
    {
        RestoreOriginalMaterials();
    }
}