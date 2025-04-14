using UnityEngine;

/// <summary>
/// 투명 영역 생성 및 관리
/// </summary>
public class TransparentZoneManager
{
    private Transform cameraTransform;
    private Transform playerTransform;  // 플레이어 참조 추가
    private GameObject transparentZonePrefab;
    private float widthFactor;
    private float heightFactor;
    private float distanceOffset;

    private GameObject transparentZone;
    public GameObject TransparentZone => transparentZone;

    private float prevWidthFactor;
    private float prevHeightFactor;
    private float prevDistanceOffset;
    private Color prevZoneColor;
    private Color zoneColor;
    private VisualizationManager visualizationManager;

    public TransparentZoneManager(Transform cameraTransform, Transform playerTransform, GameObject prefab, 
                                 float widthFactor, float heightFactor, float distanceOffset)
    {
        this.cameraTransform = cameraTransform;
        this.playerTransform = playerTransform;  // 플레이어 참조 저장
        this.transparentZonePrefab = prefab;
        this.widthFactor = widthFactor;
        this.heightFactor = heightFactor;
        this.distanceOffset = distanceOffset;
    }

    public void CreateTransparentZone()
    {
        transparentZone = transparentZonePrefab != null ? 
            Object.Instantiate(transparentZonePrefab) : CreateDefaultZone();
        transparentZone.transform.SetParent(cameraTransform, false);
        transparentZone.transform.localPosition = Vector3.zero;
        transparentZone.transform.localRotation = Quaternion.identity;
    }

    private GameObject CreateDefaultZone()
    {
        GameObject zone = new GameObject("TransparentZone");
        MeshFilter mf = zone.AddComponent<MeshFilter>();
        MeshRenderer mr = zone.AddComponent<MeshRenderer>();

        GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mf.sharedMesh = tempCube.GetComponent<MeshFilter>().sharedMesh;
        Object.Destroy(tempCube);

        return zone;
    }

    public void UpdateTransparentZone(float floorDistance, float requiredHeight)
    {
        if (!transparentZone || !cameraTransform || !playerTransform) return;

        Camera cam = cameraTransform.GetComponent<Camera>();
        if (!cam) return;

        // 실제 카메라-플레이어 거리 계산 (수정)
        float playerDistance = Vector3.Distance(cameraTransform.position, playerTransform.position);
        
        // FOV 기반 너비 계산
        float width = CalculateCameraWidthAtDistance(cam, playerDistance);
        
        // 투명 영역 위치 설정 (수정)
        // 카메라와 플레이어 사이의 중간 지점에 위치
        transparentZone.transform.localPosition = new Vector3(0, 0, playerDistance * 0.5f);
        
        // 투명 영역 크기 설정 (수정)
        transparentZone.transform.localScale = new Vector3(
            width * widthFactor,             // 너비 (FOV 기반)
            requiredHeight * heightFactor,   // 높이 (천장 높이 기반)
            playerDistance + distanceOffset  // 길이 (플레이어 거리 + 오프셋으로 수정)
        );
        
        // 디버그 로그 추가
        //Debug.Log($"TransparentZone 업데이트: 위치={transparentZone.transform.position}, 크기={transparentZone.transform.localScale}");
    }

    private float CalculateCameraWidthAtDistance(Camera cam, float distance)
    {
        if (cam.orthographic)
            return cam.orthographicSize * 2f * cam.aspect;

        float halfFov = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;
        return 2f * distance * Mathf.Tan(halfFov) * cam.aspect;
    }

    /// <summary>
    /// 설정 값 업데이트
    /// </summary>
    public void UpdateSettings(float widthFactor, float heightFactor, float distanceOffset)
    {
        this.widthFactor = widthFactor;
        this.heightFactor = heightFactor;
        this.distanceOffset = distanceOffset;
        
        // 설정 변경 후 즉시 적용
        if (transparentZone != null && cameraTransform != null && playerTransform != null)
        {
            // 현재 값으로 즉시 업데이트
            // 실제 거리와 높이는 다음 Update 호출에서 계산됨
            UpdateTransparentZone(
                Vector3.Distance(cameraTransform.position, playerTransform.position), 
                CalculateCurrentHeight()
            );
        }
    }

    /// <summary>
    /// 현재 상태에서 높이 값 계산
    /// </summary>
    private float CalculateCurrentHeight()
    {
        if (transparentZone != null)
        {
            // 현재 설정된 스케일에서 역산
            return transparentZone.transform.localScale.y / heightFactor;
        }
        
        // 기본값
        return Vector3.Distance(cameraTransform.position, playerTransform.position) * 2f;
    }

    /// <summary>
    /// 설정 변경사항 감지 및 업데이트
    /// </summary>
    private void UpdateSettingsIfChanged()
    {
        bool settingsChanged = false;
        
        // 너비 인자 변경 감지
        if (prevWidthFactor != widthFactor)
        {
            settingsChanged = true;
            prevWidthFactor = widthFactor;
        }
        
        // 높이 인자 변경 감지
        if (prevHeightFactor != heightFactor)
        {
            settingsChanged = true;
            prevHeightFactor = heightFactor;
        }
        
        // 거리 오프셋 변경 감지
        if (prevDistanceOffset != distanceOffset)
        {
            settingsChanged = true;
            prevDistanceOffset = distanceOffset;
        }
        
        // 영역 색상 변경 감지
        if (prevZoneColor != zoneColor)
        {
            settingsChanged = true;
            prevZoneColor = zoneColor;
            
            // 시각화 매니저에 새 색상 업데이트
            visualizationManager.UpdateZoneColor(zoneColor);
        }
        
        // 변경된 경우 설정 업데이트
        if (settingsChanged)
        {
            UpdateSettings(widthFactor, heightFactor, distanceOffset);
        }
    }
}