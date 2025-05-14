using UnityEngine;
using UnityEngine.UI;

public class OptionToggleUI : MonoBehaviour
{
    public GameObject optionsPanel; // 옵션창 오브젝트
    public Button optionButton;     // 옵션 버튼

    void Start()
    {
        // 초기에는 옵션창 숨김
        optionsPanel.SetActive(false);

        // 버튼 클릭 시 Toggle 호출
        optionButton.onClick.AddListener(ToggleOptionsPanel);
    }

    void ToggleOptionsPanel()
    {
        // 현재 상태를 반전
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }
}