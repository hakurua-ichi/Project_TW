// using UnityEngine;
// using UnityEngine.Rendering;
// using UnityEngine.Rendering.Universal;
// using System;
// using System.IO;

// [Serializable]
// public class CameraSettings
// {
//     public bool postProcessingEnabled = true;
//     public AntialiasingQuality antialiasingQuality = AntialiasingQuality.Medium;
//     public bool shadowsEnabled = true;
//     public bool occlusionCullingEnabled = true;
//     public float soundVolume = 1.0f;
//     public int antialiasingMode = 1; // 0: None, 1: FXAA, 2: SMAA, 3: TAA
//     public int shadowResolution = 1; // 0: Low, 1: Medium, 2: High
// }

// public enum AntialiasingQuality
// {
//     None,
//     Low,
//     Medium,
//     High
// }

// /// <summary>
// /// 카메라 옵션 시스템 - 그래픽 및 카메라 설정을 관리합니다.
// /// </summary>
// public class CameraOption_System : MonoBehaviour
// {
//     #region 변수 선언
    
//     private Camera mainCamera;
//     private Volume postProcessingVolume;
    
//     [SerializeField]
//     private CameraSettings settings = new CameraSettings();

//     // 싱글톤 인스턴스
//     public static CameraOption_System Instance { get; private set; }

//     private string settingsFilePath;

//     [Header("디버그 옵션")]
//     [SerializeField] private bool debugMode = false;
//     [SerializeField] private KeyCode saveSettingsKey = KeyCode.F5;
//     [SerializeField] private KeyCode loadSettingsKey = KeyCode.F7;
//     [SerializeField] private KeyCode resetSettingsKey = KeyCode.F12;

//     #endregion

//     #region Unity 생명주기 메서드

//     private void Awake()
//     {
//         InitializeSingleton();
//     }
    
//     private void Start()
//     {
//         InitializeComponents();
//         ApplySettings();
//     }

//     private void Update()
//     {
//         HandleDebugInputs();
//     }

//     #endregion

//     #region 초기화 메서드

//     private void InitializeSingleton()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//             settingsFilePath = Path.Combine(Application.persistentDataPath, "cameraSettings.json");
//             LoadSettings();
//         }
//         else if (Instance != this)
//         {
//             Debug.Log("CameraOption_System 인스턴스가 이미 존재합니다. 중복 객체를 제거합니다.");
//             Destroy(gameObject);
//         }
//     }

//     private void InitializeComponents()
//     {
//         // 메인 카메라 초기화
//         InitializeMainCamera();
        
//         // 포스트 프로세싱 볼륨 초기화
//         InitializePostProcessingVolume();
//     }
    
//     private void InitializeMainCamera()
//     {
//         mainCamera = Camera.main;
//         if (mainCamera == null)
//         {
//             Debug.LogError("Main Camera not found. Please ensure there is a camera with the 'MainCamera' tag.");
//         }
//     }
    
//     private void InitializePostProcessingVolume()
//     {
//         // 기존 볼륨 찾기
//         postProcessingVolume = FindFirstObjectByType<Volume>();
        
//         // 볼륨이 없으면 생성
//         if (postProcessingVolume == null && mainCamera != null)
//         {
//             CreatePostProcessingVolume();
//         }
//     }
    
//     private void CreatePostProcessingVolume()
//     {
//         postProcessingVolume = gameObject.AddComponent<Volume>();
//         postProcessingVolume.isGlobal = true;
//         postProcessingVolume.profile = ScriptableObject.CreateInstance<VolumeProfile>();
        
//         if (settings.postProcessingEnabled)
//         {
//             AddDefaultPostProcessingEffects();
//         }
        
//         gameObject.name = "Camera Option System (with Post Processing)";
//         Debug.Log("Created Volume on CameraOption_System object with basic post-processing effects");
//     }
    
//     private void AddDefaultPostProcessingEffects()
//     {
//         if (postProcessingVolume != null && postProcessingVolume.profile != null)
//         {
//             // 블룸 효과 추가
//             Bloom bloom = postProcessingVolume.profile.Add<Bloom>(true);
//             bloom.intensity.Override(0.5f);
            
//             // 색상 조정 효과 추가
//             ColorAdjustments colorAdj = postProcessingVolume.profile.Add<ColorAdjustments>(true);
//             colorAdj.contrast.Override(10f);
//         }
//     }

//     #endregion

//     #region 설정 적용 메서드

//     /// <summary>
//     /// 현재 설정을 카메라 및 렌더링 시스템에 적용합니다.
//     /// </summary>
//     public void ApplySettings()
//     {
//         if (mainCamera == null)
//         {
//             InitializeMainCamera();
//             if (mainCamera == null) return;
//         }

//         ApplyPostProcessingSettings();
//         ConfigureAntialiasing();
//         ApplyShadowSettings();
//         ApplyOtherSettings();
        
//         SaveSettings();
//     }
    
//     private void ApplyPostProcessingSettings()
//     {
//         if (postProcessingVolume == null) return;
        
//         // Volume 컴포넌트 활성화 여부 설정
//         postProcessingVolume.enabled = settings.postProcessingEnabled;
        
//         // 카메라 포스트 프로세싱 설정
//         UniversalAdditionalCameraData cameraData = mainCamera.GetUniversalAdditionalCameraData();
//         if (cameraData != null)
//         {
//             cameraData.renderPostProcessing = settings.postProcessingEnabled;
//         }
        
//         // Volume 프로필 없으면 생성
//         if (postProcessingVolume.profile == null)
//         {
//             postProcessingVolume.profile = ScriptableObject.CreateInstance<VolumeProfile>();
//         }
        
//         // 기본 효과 추가 (없는 경우)
//         if (settings.postProcessingEnabled && !postProcessingVolume.profile.Has<Bloom>())
//         {
//             AddDefaultPostProcessingEffects();
//         }
//     }
    
//     private void ApplyShadowSettings()
//     {
//         // 그림자 활성화 여부
//         QualitySettings.shadows = settings.shadowsEnabled 
//             ? UnityEngine.ShadowQuality.All 
//             : UnityEngine.ShadowQuality.Disable;
        
//         // 그림자 해상도 설정
//         QualitySettings.shadowResolution = (UnityEngine.ShadowResolution)settings.shadowResolution;
//     }
    
//     private void ApplyOtherSettings()
//     {
//         // 오클루전 컬링 설정
//         if (mainCamera != null)
//         {
//             mainCamera.useOcclusionCulling = settings.occlusionCullingEnabled;
//         }
        
//         // 사운드 볼륨 설정
//         AudioListener.volume = settings.soundVolume;
//     }
    
//     private void ConfigureAntialiasing()
//     {
//         if (mainCamera == null) return;

//         UniversalAdditionalCameraData cameraData = mainCamera.GetUniversalAdditionalCameraData();
//         if (cameraData != null)
//         {
//             // 안티앨리어싱 모드 설정
//             cameraData.antialiasing = (AntialiasingMode)settings.antialiasingMode;
            
//             // 안티앨리어싱 품질 설정
//             AntialiasingQuality quality = settings.antialiasingQuality;
//             if (quality == AntialiasingQuality.None)
//             {
//                 cameraData.antialiasing = AntialiasingMode.None;
//                 cameraData.antialiasingQuality = UnityEngine.Rendering.Universal.AntialiasingQuality.Low;
//             }
//             else
//             {
//                 cameraData.antialiasingQuality = (UnityEngine.Rendering.Universal.AntialiasingQuality)quality;
//             }
//         }
//         else
//         {
//             Debug.LogWarning("Universal Additional Camera Data not found. Make sure you're using URP.");
//         }
//     }

//     #endregion

//     #region 설정 저장 및 로드

//     /// <summary>
//     /// 현재 설정을 파일에 저장합니다.
//     /// </summary>
//     public void SaveSettings()
//     {
//         try
//         {
//             string json = JsonUtility.ToJson(settings, true);
//             File.WriteAllText(settingsFilePath, json);
            
//             if (debugMode)
//             {
//                 Debug.Log("Camera settings saved to: " + settingsFilePath);
//             }
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError("Error saving camera settings: " + e.Message);
//         }
//     }

//     /// <summary>
//     /// 파일에서 설정을 로드합니다.
//     /// </summary>
//     public void LoadSettings()
//     {
//         if (File.Exists(settingsFilePath))
//         {
//             try 
//             {
//                 string json = File.ReadAllText(settingsFilePath);
//                 settings = JsonUtility.FromJson<CameraSettings>(json);
                
//                 if (debugMode)
//                 {
//                     Debug.Log("Camera settings loaded from: " + settingsFilePath);
//                 }
                
//                 ApplySettings();
//             }
//             catch (System.Exception e)
//             {
//                 Debug.LogError("Error loading camera settings: " + e.Message);
//                 UseDefaultSettings();
//             }
//         }
//         else
//         {
//             if (debugMode)
//             {
//                 Debug.Log("Settings file not found. Using default settings.");
//             }
//             UseDefaultSettings();
//         }
//     }
    
//     private void UseDefaultSettings()
//     {
//         settings = new CameraSettings();
//         ApplySettings();
//     }

//     /// <summary>
//     /// 설정을 기본값으로 리셋합니다.
//     /// </summary>
//     public void ResetToDefaults()
//     {
//         UseDefaultSettings();
//     }

//     #endregion

//     #region 디버그 메서드

//     private void HandleDebugInputs()
//     {
//         if (!debugMode) return;
        
//         if (Input.GetKeyDown(saveSettingsKey))
//         {
//             SaveSettings();
//             Debug.Log("[CameraOption_System] Settings saved manually with key: " + saveSettingsKey);
//         }
        
//         if (Input.GetKeyDown(loadSettingsKey))
//         {
//             LoadSettings();
//             Debug.Log("[CameraOption_System] Settings loaded manually with key: " + loadSettingsKey);
//         }
        
//         if (Input.GetKeyDown(resetSettingsKey))
//         {
//             ResetToDefaults();
//             Debug.Log("[CameraOption_System] Settings reset to defaults with key: " + resetSettingsKey);
//         }
        
//         if (Input.GetKeyDown(KeyCode.F1))
//         {
//             PrintDebugInfo();
//         }
//     }
    
//     private void PrintDebugInfo()
//     {
//         Debug.Log("=== Camera Option System - Current Settings ===");
//         Debug.Log($"Post Processing: {settings.postProcessingEnabled}");
//         Debug.Log($"Antialiasing Mode: {(AntialiasingMode)settings.antialiasingMode}");
//         Debug.Log($"Antialiasing Quality: {settings.antialiasingQuality}");
//         Debug.Log($"Shadows Enabled: {settings.shadowsEnabled}");
//         Debug.Log($"Shadow Resolution: {(UnityEngine.ShadowResolution)settings.shadowResolution}");
//         Debug.Log($"Occlusion Culling: {settings.occlusionCullingEnabled}");
//         Debug.Log($"Sound Volume: {settings.soundVolume}");
//         Debug.Log($"Settings File Path: {settingsFilePath}");
        
//         if (postProcessingVolume != null)
//         {
//             Debug.Log($"Volume Component: Active = {postProcessingVolume.enabled}, Profile = {(postProcessingVolume.profile != null ? postProcessingVolume.profile.name : "None")}");
//         }
//         else
//         {
//             Debug.Log("Volume Component: Not found");
//         }
        
//         if (mainCamera != null)
//         {
//             Debug.Log($"Main Camera: {mainCamera.name}, Occlusion Culling = {mainCamera.useOcclusionCulling}");
            
//             UniversalAdditionalCameraData cameraData = mainCamera.GetUniversalAdditionalCameraData();
//             if (cameraData != null)
//             {
//                 Debug.Log($"URP Camera Data: Antialiasing = {cameraData.antialiasing}, Quality = {cameraData.antialiasingQuality}");
//             }
//         }
//         else
//         {
//             Debug.Log("Main Camera: Not found");
//         }
        
//         Debug.Log("=== End of Debug Info ===");
//     }

//     #endregion
// }
