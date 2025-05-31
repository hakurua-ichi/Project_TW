using UnityEngine;
using UnityEngine.UI;

public class OptionToggleUI : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel; // 옵션창 오브젝트
    [SerializeField] private Button optionButton;     // 옵션 버튼
    [SerializeField] private Button optionButton2;    // 옵션 버튼2 옵션 안에서 종료하기위한 버튼

    void Start()
    {
        // 초기에는 옵션창 숨김
        optionsPanel.SetActive(false);
        if (optionButton == null)
        {
            // 이 친구는 없으면 진짜 잘못된거임.
            Debug.LogWarning("옵션 버튼이 설정되지 않았습니다.");
        }
        // 버튼 클릭 시 Toggle 호출
        if (optionButton != null) { optionButton.onClick.AddListener(ToggleOptionsPanel); }
        if (optionButton2 != null) { optionButton2.onClick.AddListener(ToggleOptionsPanel); }
    }

    void ToggleOptionsPanel()
    {
        // 현재 상태를 반전
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }
}