using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace DevProjectSettings.Camera.Option
{
    /// <summary>
    /// 포스트 프로세싱 관련 기능을 관리하는 클래스
    /// </summary>
    /// <remarks>
    /// 기능 목록:
    /// 1. 포스트 프로세싱 초기화 - 볼륨 컴포넌트 초기화 및 생성
    /// 2. 설정 적용 - CameraSettings에 따라 포스트 프로세싱 적용
    /// 3. 기본 효과 관리 - 블룸, 색상 조정 등 기본 효과 추가
    /// 
    /// 메서드:
    /// - InitializeVolume(): 포스트 프로세싱 볼륨 초기화
    /// - CreatePostProcessingVolume(): 새로운 포스트 프로세싱 볼륨 생성
    /// - AddDefaultPostProcessingEffects(): 기본 효과(블룸, 색상 조정 등) 추가
    /// - ApplySettings(): 설정에 따라 포스트 프로세싱 활성화 및 적용
    /// - GetVolume(): 현재 포스트 프로세싱 볼륨 반환
    /// </remarks>
    public class PostProcessingManager
    {
        private UnityEngine.Camera mainCamera;
        private Volume postProcessingVolume;

        public PostProcessingManager(UnityEngine.Camera camera)
        {
            mainCamera = camera;
        }

        /// <summary>
        /// 포스트 프로세싱 볼륨을 초기화합니다.
        /// </summary>
        public void InitializeVolume(GameObject ownerObject)
        {
            // 기존 볼륨 찾기
            postProcessingVolume = Object.FindFirstObjectByType<Volume>();
            
            // 볼륨이 없으면 생성
            if (postProcessingVolume == null && mainCamera != null)
            {
                CreatePostProcessingVolume(ownerObject);
            }
        }

        /// <summary>
        /// 새 포스트 프로세싱 볼륨을 생성합니다.
        /// </summary>
        private void CreatePostProcessingVolume(GameObject ownerObject)
        {
            postProcessingVolume = ownerObject.AddComponent<Volume>();
            postProcessingVolume.isGlobal = true;
            postProcessingVolume.profile = ScriptableObject.CreateInstance<VolumeProfile>();
            
            Debug.Log("Created Volume component with empty profile");
        }

        /// <summary>
        /// 기본 포스트 프로세싱 효과를 추가합니다.
        /// </summary>
        public void AddDefaultPostProcessingEffects()
        {
            if (postProcessingVolume != null && postProcessingVolume.profile != null)
            {
                // 블룸 효과 추가
                if (!postProcessingVolume.profile.Has<Bloom>())
                {
                    Bloom bloom = postProcessingVolume.profile.Add<Bloom>(true);
                    bloom.intensity.Override(0.5f);
                }
                
                // 색상 조정 효과 추가
                if (!postProcessingVolume.profile.Has<ColorAdjustments>())
                {
                    ColorAdjustments colorAdj = postProcessingVolume.profile.Add<ColorAdjustments>(true);
                    colorAdj.contrast.Override(10f);
                }
            }
        }

        /// <summary>
        /// 설정에 따라 포스트 프로세싱을 적용합니다.
        /// </summary>
        public void ApplySettings(CameraSettings settings)
        {
            if (postProcessingVolume == null) return;
            
            // Volume 컴포넌트 활성화 여부 설정
            postProcessingVolume.enabled = settings.postProcessingEnabled;
            
            // 카메라 포스트 프로세싱 설정
            UniversalAdditionalCameraData cameraData = mainCamera.GetUniversalAdditionalCameraData();
            if (cameraData != null)
            {
                cameraData.renderPostProcessing = settings.postProcessingEnabled;
            }
            
            // Volume 프로필 없으면 생성
            if (postProcessingVolume.profile == null)
            {
                postProcessingVolume.profile = ScriptableObject.CreateInstance<VolumeProfile>();
            }
            
            // 기본 효과 추가 (없는 경우)
            if (settings.postProcessingEnabled && !postProcessingVolume.profile.Has<Bloom>())
            {
                AddDefaultPostProcessingEffects();
            }
        }
        
        public Volume GetVolume()
        {
            return postProcessingVolume;
        }
    }
}