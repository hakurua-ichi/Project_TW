using UnityEngine;

public class TextDisplayState
{
    public string MessageToDisplay { get; private set; } // 표시할 메시지
    public bool IsActive { get; set; } = false;       // 현재 텍스트 표시가 활성화되어야 하는지 여부

    public void SetMessage(string message)
    {
        MessageToDisplay = message;
    }
}