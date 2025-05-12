using System;
using UnityEngine.Rendering.Universal;

namespace DevProjectSettings.Camera.Option
{
    /// <summary>
    /// 카메라 및 그래픽 설정을 저장하는 데이터 구조
    /// </summary>
    /// <remarks>
    /// 기능 목록:
    /// 1. 카메라 설정 데이터 저장 - 포스트 프로세싱, 안티앨리어싱, 그림자 등 설정 값 보관
    /// 2. 기본 설정 제공 - 시스템 초기화 또는 리셋시 사용할 기본 설정 제공
    /// 3. 설정 복사 - 다른 설정 객체의 값을 현재 객체로 복사
    ///
    /// 주요 속성:
    /// - postProcessingEnabled: 포스트 프로세싱 활성화 여부
    /// - antialiasingQuality: 안티앨리어싱 품질(None, Low, Medium, High)
    /// - antialiasingMode: 안티앨리어싱 모드(None, FXAA, SMAA, TAA)
    /// - shadowsEnabled: 그림자 활성화 여부
    /// - shadowResolution: 그림자 해상도(Low, Medium, High)
    /// - occlusionCullingEnabled: 오클루전 컬링 활성화 여부
    /// - soundVolume: 사운드 볼륨(0.0-1.0)
    ///
    /// 메서드:
    /// - GetDefaultSettings(): 기본 설정으로 초기화된 객체 반환
    /// - CopyFrom(): 다른 설정 객체의 값을 현재 객체로 복사
    /// </remarks>
    [Serializable]
    public class CameraSettings
    {
        public bool postProcessingEnabled = true;
        public AntialiasingQuality antialiasingQuality = AntialiasingQuality.Medium;
        public bool shadowsEnabled = true;
        public bool occlusionCullingEnabled = true;
        public float soundVolume = 1.0f;
        public int antialiasingMode = 1; // 0: None, 1: FXAA, 2: SMAA, 3: TAA
        public int shadowResolution = 1; // 0: Low, 1: Medium, 2: High

        /// <summary>
        /// 기본 설정으로 초기화된 새 설정 객체를 반환합니다.
        /// </summary>
        public static CameraSettings GetDefaultSettings()
        {
            return new CameraSettings();
        }

        /// <summary>
        /// 다른 설정 객체의 값을 현재 객체에 복사합니다.
        /// </summary>
        public void CopyFrom(CameraSettings other)
        {
            postProcessingEnabled = other.postProcessingEnabled;
            antialiasingQuality = other.antialiasingQuality;
            shadowsEnabled = other.shadowsEnabled;
            occlusionCullingEnabled = other.occlusionCullingEnabled;
            soundVolume = other.soundVolume;
            antialiasingMode = other.antialiasingMode;
            shadowResolution = other.shadowResolution;
        }
    }

    /// <summary>
    /// 안티앨리어싱 품질 설정
    /// </summary>
    public enum AntialiasingQuality
    {
        None,
        Low,
        Medium,
        High
    }
}