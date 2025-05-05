using UnityEngine;
using System.IO;

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