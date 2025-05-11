// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;

// // 가제작 코드, 카메라 옵션 UI 어댑터, 후에 다시 작성해야함.
// // 필요시 그대로 사용해도 무방.
// public class CameraOptionUI_Adapter : MonoBehaviour
// {
//     // UI 요소들
//     [Header("Toggle Elements")]
//     [SerializeField] private Toggle postProcessingToggle;
//     [SerializeField] private Toggle shadowsToggle;
//     [SerializeField] private Toggle occlusionCullingToggle;

//     [Header("Dropdown Elements")]
//     [SerializeField] private TMP_Dropdown antialiasingDropdown;
//     [SerializeField] private TMP_Dropdown shadowResolutionDropdown;

//     [Header("Slider Elements")]
//     [SerializeField] private Slider soundVolumeSlider;
//     [SerializeField] private TextMeshProUGUI volumePercentText;

//     // 카메라 옵션 시스템 참조
//     private CameraOption_System cameraOptionSystem;

//     private void Start()
//     {
//         // 카메라 옵션 시스템 찾기
//         cameraOptionSystem = CameraOption_System.Instance;
        
//         if (cameraOptionSystem == null)
//         {
//             Debug.LogError("CameraOption_System not found in the scene.");
//             return;
//         }
        
//         // UI 요소 설정
//         SetupUI();
        
//         // 이벤트 리스너 등록
//         RegisterEventListeners();
        
//         // 현재 설정으로 UI 업데이트
//         UpdateUIFromSettings();
//     }
    
//     // UI 드롭다운 옵션 등 초기 설정
//     private void SetupUI()
//     {
//         // 안티앨리어싱 드롭다운 옵션 설정
//         if (antialiasingDropdown != null)
//         {
//             antialiasingDropdown.ClearOptions();
//             antialiasingDropdown.AddOptions(new System.Collections.Generic.List<string> {
//                 "사용 안함", "FXAA", "SMAA", "TAA"
//             });
//         }
        
//         // 그림자 해상도 드롭다운 설정
//         if (shadowResolutionDropdown != null)
//         {
//             shadowResolutionDropdown.ClearOptions();
//             shadowResolutionDropdown.AddOptions(new System.Collections.Generic.List<string> {
//                 "낮음", "중간", "높음"
//             });
//         }
//     }
    
//     // UI 요소에 이벤트 리스너 등록
//     private void RegisterEventListeners()
//     {
//         if (postProcessingToggle != null)
//             postProcessingToggle.onValueChanged.AddListener(OnPostProcessingToggleChanged);
        
//         if (shadowsToggle != null)
//             shadowsToggle.onValueChanged.AddListener(OnShadowsToggleChanged);
        
//         if (occlusionCullingToggle != null)
//             occlusionCullingToggle.onValueChanged.AddListener(OnOcclusionCullingToggleChanged);
        
//         if (antialiasingDropdown != null)
//             antialiasingDropdown.onValueChanged.AddListener(OnAntialiasingDropdownChanged);
        
//         if (shadowResolutionDropdown != null)
//             shadowResolutionDropdown.onValueChanged.AddListener(OnShadowResolutionDropdownChanged);
        
//         if (soundVolumeSlider != null)
//             soundVolumeSlider.onValueChanged.AddListener(OnSoundVolumeSliderChanged);
//     }
    
//     // 설정에 따라 UI 업데이트
//     public void UpdateUIFromSettings()
//     {
//         if (cameraOptionSystem == null) return;
        
//         CameraSettings settings = cameraOptionSystem.GetCurrentSettings();
        
//         if (postProcessingToggle != null)
//             postProcessingToggle.isOn = settings.postProcessingEnabled;
        
//         if (shadowsToggle != null)
//             shadowsToggle.isOn = settings.shadowsEnabled;
        
//         if (occlusionCullingToggle != null)
//             occlusionCullingToggle.isOn = settings.occlusionCullingEnabled;
        
//         if (antialiasingDropdown != null)
//             antialiasingDropdown.value = settings.antialiasingMode;
        
//         if (shadowResolutionDropdown != null)
//             shadowResolutionDropdown.value = settings.shadowResolution;
        
//         if (soundVolumeSlider != null)
//             soundVolumeSlider.value = settings.soundVolume;
        
//         UpdateVolumeText(settings.soundVolume);
//     }

//     // 이벤트 핸들러
//     private void OnPostProcessingToggleChanged(bool value)
//     {
//         cameraOptionSystem.SetPostProcessing(value);
//     }

//     private void OnShadowsToggleChanged(bool value)
//     {
//         cameraOptionSystem.SetShadows(value);
//     }

//     private void OnOcclusionCullingToggleChanged(bool value)
//     {
//         cameraOptionSystem.SetOcclusionCulling(value);
//     }

//     private void OnAntialiasingDropdownChanged(int value)
//     {
//         cameraOptionSystem.SetAntialiasingMode(value);
//     }

//     private void OnShadowResolutionDropdownChanged(int value)
//     {
//         cameraOptionSystem.SetShadowResolution(value);
//     }

//     private void OnSoundVolumeSliderChanged(float value)
//     {
//         cameraOptionSystem.SetSoundVolume(value);
//         UpdateVolumeText(value);
//     }

//     private void UpdateVolumeText(float volume)
//     {
//         if (volumePercentText != null)
//         {
//             volumePercentText.text = Mathf.RoundToInt(volume * 100) + "%";
//         }
//     }

//     // 기본 설정으로 초기화 버튼
//     public void ResetToDefaults()
//     {
//         if (cameraOptionSystem != null)
//         {
//             cameraOptionSystem.ResetToDefaults();
//             UpdateUIFromSettings();
//         }
//     }
// }