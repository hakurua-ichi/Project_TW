using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace DevProjectSettings.Camera.Option
{
    /// <summary>
    /// 카메라 설정 디버그 기능을 제공하는 클래스
    /// </summary>
    /// <remarks>
    /// 기능 목록:
    /// 1. 디버그 키 감지 - 특정 키 입력에 따른 설정 저장, 로드, 리셋 기능 제공
    /// 2. 디버그 정보 표시 - 현재 카메라 설정 및 상태를 콘솔에 출력
    /// 3. 디버그 모드 관리 - 디버그 기능의 활성화/비활성화 관리
    ///
    /// 메서드:
    /// - SetReferences(): 디버그에 필요한 객체 참조 설정
    /// - SetDebugMode(): 디버그 모드 활성화/비활성화
    /// - HandleDebugInputs(): F1, F5 등 디버그 키 입력 처리
    /// - PrintDebugInfo(): 현재 설정 정보를 콘솔에 출력
    /// </remarks>
    public class CameraDebugTools
    {
        private bool debugMode;
        private readonly KeyCode saveSettingsKey;
        private readonly KeyCode loadSettingsKey;
        private readonly KeyCode resetSettingsKey;
        private readonly KeyCode debugInfoKey = KeyCode.F1;
        
        private UnityEngine.Camera mainCamera;
        private Volume postProcessingVolume;
        private string settingsFilePath;

        public CameraDebugTools(bool enableDebug, KeyCode saveKey, KeyCode loadKey, KeyCode resetKey)
        {
            debugMode = enableDebug;
            saveSettingsKey = saveKey;
            loadSettingsKey = loadKey;
            resetSettingsKey = resetKey;
        }

        /// <summary>
        /// 디버그에 필요한 참조를 설정합니다.
        /// </summary>
        public void SetReferences(UnityEngine.Camera camera, Volume volume, string filePath)
        {
            mainCamera = camera;
            postProcessingVolume = volume;
            settingsFilePath = filePath;
        }

        /// <summary>
        /// 디버그 모드를 설정합니다.
        /// </summary>
        public void SetDebugMode(bool enabled)
        {
            debugMode = enabled;
            Debug.Log($"Camera Option System - Debug Mode: {(enabled ? "Enabled" : "Disabled")}");
        }

        /// <summary>
        /// 디버그 키 입력을 처리합니다.
        /// </summary>
        public bool HandleDebugInputs(System.Action onSave, System.Action onLoad, System.Action onReset)
        {
            if (!debugMode) return false;
            
            if (Input.GetKeyDown(saveSettingsKey))
            {
                onSave?.Invoke();
                Debug.Log("[CameraOption_System] Settings saved manually with key: " + saveSettingsKey);
                return true;
            }
            
            if (Input.GetKeyDown(loadSettingsKey))
            {
                onLoad?.Invoke();
                Debug.Log("[CameraOption_System] Settings loaded manually with key: " + loadSettingsKey);
                return true;
            }
            
            if (Input.GetKeyDown(resetSettingsKey))
            {
                onReset?.Invoke();
                Debug.Log("[CameraOption_System] Settings reset to defaults with key: " + resetSettingsKey);
                return true;
            }
            
            if (Input.GetKeyDown(debugInfoKey))
            {
                PrintDebugInfo();
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// 현재 설정에 대한 디버그 정보를 출력합니다.
        /// </summary>
        public void PrintDebugInfo()
        {
            if (!debugMode || mainCamera == null) return;
            
            Debug.Log("=== Camera Option System - Debug Info ===");
            
            UniversalAdditionalCameraData cameraData = mainCamera.GetUniversalAdditionalCameraData();
            
            Debug.Log($"Main Camera: {mainCamera.name}");
            Debug.Log($"Occlusion Culling: {mainCamera.useOcclusionCulling}");
            
            if (cameraData != null)
            {
                Debug.Log($"Rendering Mode: {cameraData.renderType}");
                Debug.Log($"Post Processing: {cameraData.renderPostProcessing}");
                Debug.Log($"Anti-aliasing: {cameraData.antialiasing}");
                Debug.Log($"Anti-aliasing Quality: {cameraData.antialiasingQuality}");
            }
            
            Debug.Log($"Shadow Quality: {QualitySettings.shadows}");
            Debug.Log($"Shadow Resolution: {QualitySettings.shadowResolution}");
            
            if (postProcessingVolume != null)
            {
                Debug.Log($"Volume: {postProcessingVolume.name}, Enabled: {postProcessingVolume.enabled}");
                Debug.Log($"Profile: {(postProcessingVolume.profile != null ? "Valid" : "Missing")}");
                
                if (postProcessingVolume.profile != null)
                {
                    Debug.Log($"Has Bloom: {postProcessingVolume.profile.Has<Bloom>()}");
                    Debug.Log($"Has Color Adjustments: {postProcessingVolume.profile.Has<ColorAdjustments>()}");
                }
            }
            
            Debug.Log($"Settings File Path: {settingsFilePath}");
            Debug.Log("=== End of Debug Info ===");
        }
    }
}