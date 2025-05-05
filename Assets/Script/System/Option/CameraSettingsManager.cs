// filepath: e:\Unity\Project_Git\Project_TW\Assets\Script\System\Option\CameraSettingsManager.cs
using UnityEngine;
using System.IO;
using DevProjectSettings.Camera.Option;

/// <summary>
/// 카메라 설정 관리를 위한 정적 유틸리티 클래스
/// </summary>
/// <remarks>
/// 기능 목록:
/// 1. 설정 파일 경로 관리 - Application.persistentDataPath를 기반으로 설정 파일 경로 제공
/// 2. 설정 저장 - CameraSettings를 JSON 형식으로 파일에 저장
/// 3. 설정 로드 - 파일에서 CameraSettings를 로드, 없으면 기본값 제공
///
/// 메서드:
/// - GetSettingsFilePath(): 설정 파일 경로 반환 
/// - SaveSettings(): 카메라 설정을 JSON 파일로 저장
/// - LoadSettings(): JSON 파일에서 카메라 설정을 로드
/// </remarks>
public static class CameraSettingsManager 
{
    // 설정 파일 경로 가져오기
    public static string GetSettingsFilePath()
    {
        return Path.Combine(Application.persistentDataPath, "cameraSettings.json");
    }
    
    // 설정 저장
    public static void SaveSettings(CameraSettings settings)
    {
        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(GetSettingsFilePath(), json);
        Debug.Log("Camera settings saved to: " + GetSettingsFilePath());
    }
    
    // 설정 로드
    public static CameraSettings LoadSettings()
    {
        string filePath = GetSettingsFilePath();
        
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            CameraSettings settings = JsonUtility.FromJson<CameraSettings>(json);
            Debug.Log("Camera settings loaded from: " + filePath);
            return settings;
        }
        else
        {
            Debug.Log("Settings file not found. Using default settings.");
            return new CameraSettings();
        }
    }
}