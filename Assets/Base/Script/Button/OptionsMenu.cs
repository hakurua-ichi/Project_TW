using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public GameObject optionsPanel;
    public Slider soundSlider;
    public TMP_Text volumeValueText;

    void Start()
    {
        // 초기값 반영
        UpdateSoundValueText(soundSlider.value);

        // 슬라이더 값 변경 시 호출
        soundSlider.onValueChanged.AddListener(UpdateSoundValueText);
    }

    /*
    public Toggle postProcessingToggle;
    public Dropdown antiAliasingDropdown;
    public Toggle shadowsToggle;
    public Toggle occlusionToggle;
    public Slider soundSlider;
    public Text soundValueText;

    public UniversalRenderPipelineAsset urpAsset;

    private void Start()
    {
        // Load saved settings
        postProcessingToggle.isOn = PlayerPrefs.GetInt("PostProcessing", 1) == 1;
        antiAliasingDropdown.value = PlayerPrefs.GetInt("AntiAliasing", 1);
        shadowsToggle.isOn = PlayerPrefs.GetInt("Shadows", 1) == 1;
        occlusionToggle.isOn = PlayerPrefs.GetInt("Occlusion", 1) == 1;
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1.0f);

        // Apply to system
        ApplySettings();

        // Listeners
        postProcessingToggle.onValueChanged.AddListener((on) => SaveAndApply());
        antiAliasingDropdown.onValueChanged.AddListener((v) => SaveAndApply());
        shadowsToggle.onValueChanged.AddListener((on) => SaveAndApply());
        occlusionToggle.onValueChanged.AddListener((on) => SaveAndApply());
        soundSlider.onValueChanged.AddListener((val) => {
            soundValueText.text = Mathf.RoundToInt(val * 100f) + "%";
            SaveAndApply();
        });
    }

    void SaveAndApply()
    {
        PlayerPrefs.SetInt("PostProcessing", postProcessingToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("AntiAliasing", antiAliasingDropdown.value);
        PlayerPrefs.SetInt("Shadows", shadowsToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Occlusion", occlusionToggle.isOn ? 1 : 0);
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);

        ApplySettings();
    }

    void ApplySettings()
    {
        // Post Processing (예: Volume 컴포넌트 켜기)
        var volume = FindObjectOfType<UnityEngine.Rendering.Volume>();
        if (volume != null)
            volume.enabled = postProcessingToggle.isOn;

        // Anti-Aliasing
        if (urpAsset != null)
        {
            switch (antiAliasingDropdown.value)
            {
                case 0: urpAsset.msaaSampleCount = 1; break; // None
                case 1: urpAsset.msaaSampleCount = 2; break;
                case 2: urpAsset.msaaSampleCount = 4; break;
            }
        }

        // Shadows
        QualitySettings.shadows = shadowsToggle.isOn ? ShadowQuality.All : ShadowQuality.Disable;

        // Occlusion Culling
        Camera.main.useOcclusionCulling = occlusionToggle.isOn;

        // Sound Volume
        AudioListener.volume = soundSlider.value;
    }
    */

    void UpdateSoundValueText(float value)
    {
        int percent = Mathf.RoundToInt(value * 100);
        volumeValueText.text = percent + "%";
    }

    public void CloseOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("optionsPanel이 연결되지 않았습니다!");
        }
    }
}