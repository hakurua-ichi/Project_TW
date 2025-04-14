using UnityEngine;

/// <summary>
/// 벽 투명화 시스템의 메인 컨트롤러
/// </summary>
public class WallTransparencySystem : MonoBehaviour
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

    // 이전 값 캐싱 변수들 (변경 감지용)
    private float prevHeightFactor;
    private float prevWidthFactor;
    private float prevDistanceOffset;
    private Color prevZoneColor;

    // 서브시스템
    private TransparentZoneManager zoneManager;
    private FloorDetector floorDetector;
    private WallDetector wallDetector;
    private MaterialApplier materialApplier;
    private VisualizationManager visualizationManager;

    void Awake()
    {
        // 서브시스템 초기화
        zoneManager = new TransparentZoneManager(cameraTrans, player, transparentZonePrefab, 
                                           zoneWidthFactor, zoneHeightFactor, zoneDistanceOffset);
        floorDetector = new FloorDetector(player, cameraTrans, wallLayers);
        wallDetector = new WallDetector(wallLayers);
        materialApplier = new MaterialApplier(transparentMaterial);
        visualizationManager = new VisualizationManager(visualizeZone, zoneColor);
        
        // 초기값 저장
        CacheCurrentValues();
    }

    void Start()
    {
        // 투명 영역 생성 및 시각화 초기화
        zoneManager.CreateTransparentZone();
        visualizationManager.SetupVisualization(zoneManager.TransparentZone);
    }

    void Update()
    {
        // 설정 변경 감지 및 업데이트
        UpdateSettingsIfChanged();
        
        // 시각화 상태 업데이트
        visualizationManager.UpdateVisualization(visualizeZone);

        // 바닥 감지 및 투명 영역 업데이트
        float floorDistance = floorDetector.CalculateFloorEdgeDistance();
        float requiredHeight = floorDetector.CalculateRequiredZoneHeight();
        
        // 투명 영역 업데이트
        zoneManager.UpdateTransparentZone(floorDistance, requiredHeight);

        // 벽 감지 및 투명화 처리
        Collider[] walls = wallDetector.DetectWalls(zoneManager.TransparentZone);
        materialApplier.ApplyTransparency(walls, floorDetector.GetExcludedFloorObjects());

        // 시각화 업데이트
        visualizationManager.UpdateVisualElements(cameraTrans.position, player.position);
    }
    
    /// <summary>
    /// 설정 변경사항 감지 및 업데이트
    /// </summary>
    private void UpdateSettingsIfChanged()
    {
        bool settingsChanged = false;
        
        // 너비 인자 변경 감지
        if (prevWidthFactor != zoneWidthFactor)
        {
            settingsChanged = true;
            //Debug.Log($"Width Factor 변경: {prevWidthFactor} → {zoneWidthFactor}");
            prevWidthFactor = zoneWidthFactor;
        }
        
        // 높이 인자 변경 감지
        if (prevHeightFactor != zoneHeightFactor)
        {
            settingsChanged = true;
            //Debug.Log($"Height Factor 변경: {prevHeightFactor} → {zoneHeightFactor}");
            prevHeightFactor = zoneHeightFactor;
        }
        
        // 거리 오프셋 변경 감지
        if (prevDistanceOffset != zoneDistanceOffset)
        {
            settingsChanged = true;
            //Debug.Log($"Distance Offset 변경: {prevDistanceOffset} → {zoneDistanceOffset}");
            prevDistanceOffset = zoneDistanceOffset;
        }
        
        // 영역 색상 변경 감지
        if (prevZoneColor != zoneColor)
        {
            settingsChanged = true;
            //Debug.Log($"Zone Color 변경: {prevZoneColor} → {zoneColor}");
            prevZoneColor = zoneColor;
            
            // 시각화 매니저에 새 색상 업데이트
            visualizationManager.UpdateZoneColor(zoneColor);
        }
        
        // 변경된 경우 설정 업데이트
        if (settingsChanged)
        {
            UpdateSubsystemSettings();
        }
    }
    
    /// <summary>
    /// 하위 시스템 설정 업데이트
    /// </summary>
    private void UpdateSubsystemSettings()
    {
        // TransparentZoneManager 설정 업데이트
        zoneManager.UpdateSettings(zoneWidthFactor, zoneHeightFactor, zoneDistanceOffset);
    }
    
    /// <summary>
    /// 현재 값 캐싱
    /// </summary>
    private void CacheCurrentValues()
    {
        prevWidthFactor = zoneWidthFactor;
        prevHeightFactor = zoneHeightFactor;
        prevDistanceOffset = zoneDistanceOffset;
        prevZoneColor = zoneColor;
    }

    void OnDisable()
    {
        materialApplier.RestoreOriginalMaterials();
    }
}