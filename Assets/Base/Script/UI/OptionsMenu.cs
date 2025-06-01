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

    public void RestartGame()
    {
        // 현재 씬을 다시 로드하여 게임을 재시작
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentScene);
    }

    public void QuitGame()
    {
        
    }
}