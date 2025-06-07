using UnityEngine;

public class TextDisplayAction : IGimmickAction
{
    private TextDisplayState textDisplayState;
    private StateText stateTextUI; // StateText UI 컴포넌트 참조

    public TextDisplayAction(TextDisplayState state, StateText ui)
    {
        textDisplayState = state;
        stateTextUI = ui;
    }

    public void Setup()
    {
    }

    public void Exit()
    {
    }

    public void Action() // 버튼 클릭 시 호출될 액션
    {
        if (stateTextUI == null)
        {
            Debug.LogWarning("TextDisplayAction: StateText UI가 할당되지 않았습니다.");
            return;
        }

        if (string.IsNullOrEmpty(textDisplayState.MessageToDisplay))
        {
            Debug.LogWarning("TextDisplayAction: 표시할 메시지가 없습니다.");
            stateTextUI.UnVisible(); // 메시지 없으면 숨김
            textDisplayState.IsActive = false;
            return;
        }

        // 현재 텍스트가 보이는 상태면 숨기고, 아니면 보이게 토글
        if (textDisplayState.IsActive)
        {
            stateTextUI.UnVisible();
            textDisplayState.IsActive = false;
            Debug.Log("텍스트 숨김.");
        }
        else
        {
            stateTextUI.SetText(true, textDisplayState.MessageToDisplay);
            textDisplayState.IsActive = true;
            Debug.Log($"텍스트 표시: {textDisplayState.MessageToDisplay}");
        }
    }
}