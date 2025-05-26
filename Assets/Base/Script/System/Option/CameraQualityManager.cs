using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace DevProjectSettings.Camera.Option
{
    /// <summary>
    /// 카메라 품질 설정을 관리하는 클래스
    /// </summary>
    /// <remarks>
    /// 기능 목록:
    /// 1. 안티앨리어싱 설정 - 안티앨리어싱 모드 및 품질 설정 적용
    /// 2. 그림자 설정 - 그림자 활성화 및 해상도 설정
    /// 3. 오클루전 컬링 - 카메라의 오클루전 컬링 설정 적용
    ///
    /// 메서드:
    /// - ConfigureAntialiasing(): 안티앨리어싱 설정(모드, 품질) 적용
    /// - ApplyShadowSettings(): 그림자 설정(활성화, 해상도) 적용
    /// - ApplyOcclusionCulling(): 오클루전 컬링 설정 적용
    /// </remarks>
    public class CameraQualityManager
    {
        private UnityEngine.Camera mainCamera;

        public CameraQualityManager(UnityEngine.Camera camera)
        {
            mainCamera = camera;
        }

        /// <summary>
        /// 안티앨리어싱 설정을 적용합니다.
        /// </summary>
        /// <param name="settings">카메라 설정 객체</param>
        /// <remarks>
        /// 이 메서드는 Universal Render Pipeline(URP)에서만 작동합니다.
        /// </remarks>
        public void ConfigureAntialiasing(CameraSettings settings)
        {
            if (mainCamera == null) return;

            UniversalAdditionalCameraData cameraData = mainCamera.GetUniversalAdditionalCameraData();
            if (cameraData != null)
            {
                // 안티앨리어싱 모드 설정
                cameraData.antialiasing = (AntialiasingMode)settings.antialiasingMode;
                
                // 안티앨리어싱 품질 설정
                if (settings.antialiasingQuality == AntialiasingQuality.None)
                {
                    cameraData.antialiasing = AntialiasingMode.None;
                    cameraData.antialiasingQuality = UnityEngine.Rendering.Universal.AntialiasingQuality.Low;
                }
                else
                {
                    cameraData.antialiasingQuality = (UnityEngine.Rendering.Universal.AntialiasingQuality)settings.antialiasingQuality;
                }
                
                Debug.Log($"Applied antialiasing: Mode={cameraData.antialiasing}, Quality={cameraData.antialiasingQuality}");
            }
            else
            {
                Debug.LogWarning("Universal Additional Camera Data not found. Make sure you're using URP.");
            }
        }

        /// <summary>
        /// 그림자 설정을 적용합니다.
        /// </summary>
        /// <param name="settings">카메라 설정 객체</param>
        /// <remarks>
        /// 그림자 활성화 여부와 해상도를 설정합니다.
        /// </remarks>
        public void ApplyShadowSettings(CameraSettings settings)
        {
            // 그림자 활성화 여부
            QualitySettings.shadows = settings.shadowsEnabled 
                ? UnityEngine.ShadowQuality.All 
                : UnityEngine.ShadowQuality.Disable;
            
            // 그림자 해상도 설정
            QualitySettings.shadowResolution = (UnityEngine.ShadowResolution)settings.shadowResolution;
        }

        /// <summary>
        /// 오클루전 컬링 설정을 적용합니다.
        /// </summary>
        /// <param name="enabled">오클루전 컬링 활성화 여부</param>
        /// <remarks>
        /// 카메라의 오클루전 컬링 설정을 활성화하거나 비활성화합니다.
        /// </remarks>
        public void ApplyOcclusionCulling(bool enabled)
        {
            if (mainCamera != null)
            {
                mainCamera.useOcclusionCulling = enabled;
            }
        }
    }
}