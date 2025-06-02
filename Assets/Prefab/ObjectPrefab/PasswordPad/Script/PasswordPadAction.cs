using UnityEngine;

public class PasswordPadAction : MonoBehaviour, IGimmickAction
{
    private StateText m_Text;
    private PasswordData m_PasswordData;
    private int padIndex;  // 이 Pad가 전체 비밀번호 배열에서 몇 번째인지
    private int password;  // 이 Pad의 현재 숫자 (0~9), -1은 “아직 누르지 않음”

    /// <summary>
    /// 외부에서 호출해서 각 값들을 초기화해 줍시다.
    /// </summary>
    public void Initialize(int index, StateText stateText, PasswordData pwD)
    {
        padIndex = index;
        m_Text = stateText;
        m_PasswordData = pwD;
        password = -1;
    }

    public void Setup()
    {
        // 딱히 초기화가 필요 없으면 비워 두세요.
    }

    public void Action()
    {
        // 첫 클릭이면 0, 그 이후엔 1씩 늘리고 9 → 0 순환
        if (password < 0)
            password = 0;
        else
            password = (password + 1) % 10;

        // 숫자를 화면에 표시하려면 StateText 사용
        if (m_Text != null)
        {
            m_Text.SetText(true, password.ToString());
        }

        // PasswordData에 “나의 인덱스, 현재 숫자” 갱신
        if (m_PasswordData != null)
        {
            m_PasswordData.SetInput(padIndex, password);
        }
    }

    public void Exit()
    {
        // (필요 시) 작업이 끝났을 때
    }

    public void numberLimit(int passwordNum)
    {
        // (필요 시) 숫자 제한 기능 구현
    }
}
