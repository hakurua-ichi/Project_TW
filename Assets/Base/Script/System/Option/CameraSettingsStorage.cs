using UnityEngine;
using System.IO;

namespace DevProjectSettings.Camera.Option
{
    /// <summary>
    /// 카메라 설정의 저장과 불러오기를 담당하는 클래스
    /// </summary>
    /// <remarks>
    /// 기능 목록:
    /// 1. 설정 저장 - CameraSettings를 JSON 형식으로 파일에 저장
    /// 2. 설정 로드 - 파일에서 CameraSettings를 로드, 없으면 기본값 반환
    /// 3. 디버그 지원 - 설정 저장 및 로드 과정의 디버그 로깅
    /// 
    /// 메서드:
    /// - SaveSettings(): 카메라 설정을 JSON 파일로 저장
    /// - LoadSettings(): JSON 파일에서 카메라 설정을 로드
    /// - UpdateDebugMode(): 디버그 모드 상태 업데이트
    /// </remarks>
    public class CameraSettingsStorage
    {
        private readonly string settingsFilePath;
        private bool debugMode;

        public CameraSettingsStorage(string filePath, bool enableDebug = false)
        {
            settingsFilePath = filePath;
            debugMode = enableDebug;
        }

        /// <summary>
        /// 디버그 모드 설정을 업데이트합니다.
        /// </summary>
        /// <param name="enableDebug">활성화할 디버그 모드 상태</param>
        public void UpdateDebugMode(bool enableDebug)
        {
            debugMode = enableDebug;
            if (debugMode)
            {
                Debug.Log("Camera settings storage debug mode updated: " + (debugMode ? "Enabled" : "Disabled"));
            }
        }

        /// <summary>
        /// 현재 설정을 파일에 저장합니다.
        /// </summary>
        /// <param name="settings">저장할 카메라 설정</param>
        public void SaveSettings(CameraSettings settings)
        {
            try
            {
                string json = JsonUtility.ToJson(settings, true);
                File.WriteAllText(settingsFilePath, json);
                
                if (debugMode)
                {
                    Debug.Log("Camera settings saved to: " + settingsFilePath);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error saving camera settings: " + e.Message);
            }
        }

        /// <summary>
        /// 파일에서 설정을 로드합니다.
        /// </summary>
        /// <returns>로드된 카메라 설정</returns>
        public CameraSettings LoadSettings()
        {
            if (File.Exists(settingsFilePath))
            {
                try 
                {
                    string json = File.ReadAllText(settingsFilePath);
                    CameraSettings settings = JsonUtility.FromJson<CameraSettings>(json);
                    
                    if (debugMode)
                    {
                        Debug.Log("Camera settings loaded from: " + settingsFilePath);
                    }
                    
                    return settings;
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error loading camera settings: " + e.Message);
                }
            }
            else if (debugMode)
            {
                Debug.Log("Settings file not found at: " + settingsFilePath + ". Using default settings.");
            }
            
            return CameraSettings.GetDefaultSettings();
        }
    }
}