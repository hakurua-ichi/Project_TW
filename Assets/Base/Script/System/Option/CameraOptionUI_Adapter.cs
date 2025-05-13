// filepath: e:\Unity\Project_Git\Project_TW\Assets\Script\System\Option\CameraOptionUI_Adapter.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 카메라 옵션 UI 어댑터 - 설정 UI와 CameraOption_System 간의 연결 역할
/// </summary>
/// <remarks>
/// 기능 목록:
/// 1. UI 초기화 - 드롭다운, 토글, 슬라이더 등의 UI 요소 초기 설정
/// 2. 이벤트 연결 - UI 요소의 이벤트를 CameraOption_System 메서드와 연결
/// 3. 설정 동기화 - CameraOption_System의 설정을 UI에 반영하거나 UI 변경을 설정에 적용
///
/// 메서드:
/// - SetupUI(): UI 드롭다운, 토글 등 초기 설정
/// - RegisterEventListeners(): UI 요소에 이벤트 리스너 등록 
/// - UpdateUIFromSettings(): 현재 설정에 따라 UI 업데이트
/// - 이벤트 핸들러 메서드들: OnPostProcessingToggleChanged() 등
/// - ResetToDefaults(): 설정을 기본값으로 초기화하고 UI 업데이트
/// </remarks>
// 가제작 코드, 카메라 옵션 UI 어댑터, 후에 다시 작성해야함.
// 필요시 그대로 사용해도 무방.
public class CameraOptionUI_Adapter : MonoBehaviour
{
    // UI 요소들
    [Header("Toggle Elements")]
    [SerializeField] private Toggle postProcessingToggle;
    [SerializeField] private Toggle shadowsToggle;
    [SerializeField] private Toggle occlusionCullingToggle;

    [Header("Dropdown Elements")]
    [SerializeField] private TMP_Dropdown antialiasingDropdown;
    [SerializeField] private TMP_Dropdown shadowResolutionDropdown;

    [Header("Slider Elements")]
    [SerializeField] private Slider soundVolumeSlider;
    [SerializeField] private TextMeshProUGUI volumePercentText;

    // 카메라 옵션 시스템 참조
    private CameraOption_System cameraOptionSystem;

    private void Start()
    {
        // 카메라 옵션 시스템 찾기
        cameraOptionSystem = CameraOption_System.Instance;
        
        if (cameraOptionSystem == null)
        {
            Debug.LogError("CameraOption_System not found in the scene.");
            return;
        }
        
        // UI 요소 설정
        SetupUI();
        
        // 이벤트 리스너 등록
        RegisterEventListeners();
        
        // 현재 설정으로 UI 업데이트
        UpdateUIFromSettings();
    }
    
    /// <summary>
    /// UI 드롭다운 옵션 등 초기 설정
    /// </summary>
    private void SetupUI()
    {
        // 안티앨리어싱 드롭다운 옵션 설정
        if (antialiasingDropdown != null)
        {
            antialiasingDropdown.ClearOptions();
            antialiasingDropdown.AddOptions(new System.Collections.Generic.List<string> {
                "None", "FXAA", "SMAA", "TAA"
            });
        }
        
        // 그림자 해상도 드롭다운 설정
        if (shadowResolutionDropdown != null)
        {
            shadowResolutionDropdown.ClearOptions();
            shadowResolutionDropdown.AddOptions(new System.Collections.Generic.List<string> {
                "Low", "Medium", "High"
            });
        }
    }
    
    /// <summary>
    /// UI 요소에 이벤트 리스너 등록
    /// </summary>
    private void RegisterEventListeners()
    {
        if (postProcessingToggle != null)
            postProcessingToggle.onValueChanged.AddListener(OnPostProcessingToggleChanged);
        
        if (shadowsToggle != null)
            shadowsToggle.onValueChanged.AddListener(OnShadowsToggleChanged);
        
        if (occlusionCullingToggle != null)
            occlusionCullingToggle.onValueChanged.AddListener(OnOcclusionCullingToggleChanged);
        
        if (antialiasingDropdown != null)
            antialiasingDropdown.onValueChanged.AddListener(OnAntialiasingDropdownChanged);
        
        if (shadowResolutionDropdown != null)
            shadowResolutionDropdown.onValueChanged.AddListener(OnShadowResolutionDropdownChanged);
        
        if (soundVolumeSlider != null)
            soundVolumeSlider.onValueChanged.AddListener(OnSoundVolumeSliderChanged);
    }
    
    /// <summary>
    /// 설정에 따라 UI 업데이트
    /// </summary>
    public void UpdateUIFromSettings()
    {
        if (cameraOptionSystem == null) return;
        
        DevProjectSettings.Camera.Option.CameraSettings settings = cameraOptionSystem.GetCurrentSettings();
        
        if (postProcessingToggle != null)
            postProcessingToggle.isOn = settings.postProcessingEnabled;
        
        if (shadowsToggle != null)
            shadowsToggle.isOn = settings.shadowsEnabled;
        
        if (occlusionCullingToggle != null)
            occlusionCullingToggle.isOn = settings.occlusionCullingEnabled;
        
        if (antialiasingDropdown != null)
            antialiasingDropdown.value = settings.antialiasingMode;
        
        if (shadowResolutionDropdown != null)
            shadowResolutionDropdown.value = settings.shadowResolution;
        
        if (soundVolumeSlider != null)
            soundVolumeSlider.value = settings.soundVolume;
        
        UpdateVolumeText(settings.soundVolume);
    }

    // 이벤트 핸들러
    private void OnPostProcessingToggleChanged(bool value)
    {
        cameraOptionSystem.SetPostProcessing(value);
    }

    private void OnShadowsToggleChanged(bool value)
    {
        cameraOptionSystem.SetShadows(value);
    }

    private void OnOcclusionCullingToggleChanged(bool value)
    {
        cameraOptionSystem.SetOcclusionCulling(value);
    }

    private void OnAntialiasingDropdownChanged(int value)
    {
        cameraOptionSystem.SetAntialiasingMode(value);
    }

    private void OnShadowResolutionDropdownChanged(int value)
    {
        cameraOptionSystem.SetShadowResolution(value);
    }

    private void OnSoundVolumeSliderChanged(float value)
    {
        cameraOptionSystem.SetSoundVolume(value);
        UpdateVolumeText(value);
    }

    /// <summary>
    /// 볼륨 텍스트 업데이트
    /// </summary>
    /// <param name="volume">현재 볼륨 값</param>
    private void UpdateVolumeText(float volume)
    {
        if (volumePercentText != null)
        {
            volumePercentText.text = Mathf.RoundToInt(volume * 100) + "%";
        }
    }

    /// <summary>
    /// 기본 설정으로 초기화 버튼
    /// </summary>
    public void ResetToDefaults()
    {
        if (cameraOptionSystem != null)
        {
            cameraOptionSystem.ResetToDefaults();
            UpdateUIFromSettings();
        }
    }
}