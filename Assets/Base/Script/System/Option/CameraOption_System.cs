using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;
using System.IO;
using DevProjectSettings.Camera.Option;

/// <summary>
/// 카메라 옵션 시스템 - 그래픽 및 카메라 설정 관리 클래스
/// </summary>
/// <remarks>
/// 기능 목록:
/// 1. 카메라 설정 관리 - 포스트 프로세싱, 안티앨리어싱, 그림자 등 그래픽 설정 제어
/// 2. 설정 저장/로드 - 플레이어 설정을 JSON 파일로 저장하고 불러오기
/// 3. 디버그 도구 지원 - 개발 중 설정 테스트를 위한 디버그 단축키 및 기능
/// 
/// 메서드:
/// - InitializeSingleton(): 싱글톤 패턴 초기화 및 설정 로드
/// - InitializeComponents(): 필요한 컴포넌트 및 매니저 초기화
/// - ApplySettings(): 현재 설정을 카메라 및 렌더링 시스템에 적용
/// - SaveSettings(): 현재 설정을 파일에 저장
/// - LoadSettings(): 파일에서 설정 로드
/// - ResetToDefaults(): 설정을 기본값으로 초기화
/// - SetPostProcessing(), SetAntialiasingMode(), SetShadows() 등: 개별 설정 항목 제어 메서드
/// - GetCurrentSettings(): 현재 설정 객체 반환
/// - SetDebugMode(): 디버그 모드 설정
/// </remarks>
public class CameraOption_System : MonoBehaviour
{
    #region 변수 선언
    
    [SerializeField]
    private CameraSettings settings = new CameraSettings();

    // 싱글톤 인스턴스
    public static CameraOption_System Instance { get; private set; }

    private UnityEngine.Camera mainCamera;
    private string settingsFilePath;
    private bool settingsChanged = false; // 설정 변경 여부 추적

    // 시스템 컴포넌트들
    private CameraSettingsStorage settingsStorage;
    private PostProcessingManager postProcessingManager;
    private CameraQualityManager qualityManager;
    private CameraDebugTools debugTools;

    [Header("디버그 옵션")]
    [SerializeField] private bool debugMode = false;
    [SerializeField] private KeyCode saveSettingsKey = KeyCode.F5;
    [SerializeField] private KeyCode loadSettingsKey = KeyCode.F7;
    [SerializeField] private KeyCode resetSettingsKey = KeyCode.F12;

    #endregion

    #region Unity 생명주기 메서드

    private void Awake()
    {
        InitializeSingleton();
    }
    
    private void Start()
    {
        InitializeComponents();
        // LoadSettings가 InitializeSingleton에서 이미 호출되었기 때문에 
        // 여기서는 설정을 적용만 하고 중복 로드하지 않습니다.
        ApplySettings(false);
    }

    private void Update()
    {
        // 디버그 입력 처리
        debugTools.HandleDebugInputs(SaveSettings, LoadSettings, ResetToDefaults);
    }

    #endregion

    #region 초기화 메서드

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            settingsFilePath = Path.Combine(Application.persistentDataPath, "cameraSettings.json");
            
            // 컴포넌트 생성
            settingsStorage = new CameraSettingsStorage(settingsFilePath, debugMode);
            debugTools = new CameraDebugTools(debugMode, saveSettingsKey, loadSettingsKey, resetSettingsKey);
            
            LoadSettings();
        }
        else if (Instance != this)
        {
            Debug.Log("CameraOption_System 인스턴스가 이미 존재합니다. 중복 객체를 제거합니다.");
            Destroy(gameObject);
        }
    }

    private void InitializeComponents()
    {
        // 메인 카메라 초기화
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found. Please ensure there is a camera with the 'MainCamera' tag.");
            return;
        }
        
        // 매니저 초기화
        postProcessingManager = new PostProcessingManager(mainCamera);
        postProcessingManager.InitializeVolume(gameObject);
        
        qualityManager = new CameraQualityManager(mainCamera);
        
        // 디버그 도구에 참조 설정
        debugTools.SetReferences(mainCamera, postProcessingManager.GetVolume(), settingsFilePath);
    }

    #endregion

    #region 설정 적용 메서드

    /// <summary>
    /// 현재 설정을 카메라 및 렌더링 시스템에 적용합니다.
    /// </summary>
    /// <param name="saveAfterApply">적용 후 설정을 저장할지 여부</param>
    public void ApplySettings(bool saveAfterApply = true)
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        postProcessingManager.ApplySettings(settings);
        qualityManager.ConfigureAntialiasing(settings);
        qualityManager.ApplyShadowSettings(settings);
        qualityManager.ApplyOcclusionCulling(settings.occlusionCullingEnabled);
        
        // 사운드 볼륨 설정
        AudioListener.volume = settings.soundVolume;
        
        if (saveAfterApply && settingsChanged)
        {
            SaveSettings();
            settingsChanged = false;
        }
    }

    #endregion

    #region 설정 저장 및 로드

    /// <summary>
    /// 현재 설정을 파일에 저장합니다.
    /// </summary>
    public void SaveSettings()
    {
        settingsStorage.SaveSettings(settings);
    }

    /// <summary>
    /// 파일에서 설정을 로드합니다.
    /// </summary>
    public void LoadSettings()
    {
        CameraSettings loadedSettings = settingsStorage.LoadSettings();
        settings.CopyFrom(loadedSettings);
        settingsChanged = true;
        
        // 이미 Start() 이후라면 설정을 적용
        if (mainCamera != null)
        {
            ApplySettings();
        }
    }

    /// <summary>
    /// 설정을 기본값으로 리셋합니다.
    /// </summary>
    public void ResetToDefaults()
    {
        settings = CameraSettings.GetDefaultSettings();
        settingsChanged = true;
        ApplySettings();
    }

    #endregion

    #region Public Setter 메서드
    
    public void SetPostProcessing(bool enabled)
    {
        if (settings.postProcessingEnabled != enabled)
        {
            settings.postProcessingEnabled = enabled;
            settingsChanged = true;
            ApplySettings();
        }
    }

    public void SetAntialiasingMode(int mode)
    {
        int clampedMode = Mathf.Clamp(mode, 0, 3);
        if (settings.antialiasingMode != clampedMode)
        {
            settings.antialiasingMode = clampedMode;
            settingsChanged = true;
            ApplySettings();
        }
    }

    public void SetAntialiasingQuality(DevProjectSettings.Camera.Option.AntialiasingQuality quality)
    {
        if (settings.antialiasingQuality != quality)
        {
            settings.antialiasingQuality = quality;
            settingsChanged = true;
            ApplySettings();
        }
    }

    public void SetShadows(bool enabled)
    {
        if (settings.shadowsEnabled != enabled)
        {
            settings.shadowsEnabled = enabled;
            settingsChanged = true;
            ApplySettings();
        }
    }

    public void SetShadowResolution(int resolution)
    {
        int clampedResolution = Mathf.Clamp(resolution, 0, 2);
        if (settings.shadowResolution != clampedResolution)
        {
            settings.shadowResolution = clampedResolution;
            settingsChanged = true;
            ApplySettings();
        }
    }

    public void SetOcclusionCulling(bool enabled)
    {
        if (settings.occlusionCullingEnabled != enabled)
        {
            settings.occlusionCullingEnabled = enabled;
            settingsChanged = true;
            ApplySettings();
        }
    }

    public void SetSoundVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp01(volume);
        if (Mathf.Approximately(settings.soundVolume, clampedVolume) == false)
        {
            settings.soundVolume = clampedVolume;
            settingsChanged = true;
            ApplySettings();
        }
    }

    /// <summary>
    /// 현재 설정 객체를 반환합니다.
    /// </summary>
    public CameraSettings GetCurrentSettings()
    {
        return settings;
    }
    
    /// <summary>
    /// 디버그 모드를 설정합니다.
    /// </summary>
    public void SetDebugMode(bool enabled)
    {
        if (debugMode != enabled)
        {
            debugMode = enabled;
            debugTools.SetDebugMode(enabled);
            
            // 새로 추가된 UpdateDebugMode 메서드 사용
            if (settingsStorage != null)
            {
                settingsStorage.UpdateDebugMode(debugMode);
            }
        }
    }
    
    #endregion
}
