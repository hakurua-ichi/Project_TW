using System;
using UnityEngine;

public class PasswordData : MonoBehaviour
{
    [Header("▶ Inspector에서 설정할 정답 비밀번호 (예: 3칸짜리 배열)")]
    [SerializeField] private int[] PasswordButton;

    // 런타임에 각 Pad에서 보내주는 현재 입력값을 저장
    private int[] currentInput;

    // 비밀번호가 정확히 맞았을 때 외부에 알려줄 이벤트
    public event Action OnPasswordMatched;

    // 비밀번호가 맞았다가 틀어졌을 때 외부에 알려줄 이벤트
    public event Action OnPasswordUnmatched;

    // 현재 비밀번호 일치 여부 상태
    private bool isMatched = false;

    void Awake()
    {
        // 정답 비밀번호 자리 수만큼 currentInput 배열 생성
        currentInput = new int[PasswordButton.Length];

        // 초기에는 -1로 둬서, 아직 값이 설정되지 않았음을 표시
        for (int i = 0; i < currentInput.Length; i++)
            currentInput[i] = -1;
    }

    /// <summary>
    /// 각 Pad(index)가 눌려서 값(value)이 바뀔 때마다 호출하세요.
    /// </summary>
    public void SetInput(int index, int value)
    {
        if (index < 0 || index >= currentInput.Length)
        {
            Debug.LogWarning($"PasswordData.SetInput: 잘못된 index({index}). 0~{currentInput.Length - 1}만 가능.");
            return;
        }

        currentInput[index] = value;
        CheckPasswordState();
    }

    /// <summary>
    /// 모든 Pad의 value가 채워졌으면(=-1 아님) 정답과 비교하여,
    /// 이전 상태와 비교 후 이벤트를 발생시킵니다.
    /// </summary>
    private void CheckPasswordState()
    {
        // 1) 아직 -1인 칸이 있으면 “아직 입력이 완성되지 않음” 상태
        for (int i = 0; i < currentInput.Length; i++)
        {
            if (currentInput[i] < 0)
            {
                // 만약 이전에 isMatched가 true였다면, 틀린 상태로 바뀐 것이므로 해제
                if (isMatched)
                {
                    isMatched = false;
                    OnPasswordUnmatched?.Invoke();
                }
                return;
            }
        }

        // 2) 모든 칸이 채워졌으므로 현재 입력이 정답과 같은지 검사
        bool currentlyMatches = true;
        for (int i = 0; i < PasswordButton.Length; i++)
        {
            if (currentInput[i] != PasswordButton[i])
            {
                currentlyMatches = false;
                break;
            }
        }

        // 3) 이전에 틀렸던 상태였다가 맞춘 순간 → 맞았다는 이벤트
        if (!isMatched && currentlyMatches)
        {
            isMatched = true;
            OnPasswordMatched?.Invoke();
        }
        // 4) 이전에 맞았던 상태였다가 틀린 순간 → 틀렸다는 이벤트
        else if (isMatched && !currentlyMatches)
        {
            isMatched = false;
            OnPasswordUnmatched?.Invoke();
        }
        // 5) 둘 다 false(아직 틀린 상태 유지)거나 둘 다 true(이미 맞은 상태 유지)면 아무 일도 하지 않음
    }

    /// <summary>
    /// (옵션) 게임 오버나 실패 시, 입력을 초기화하고 상태를 리셋하려면 호출하세요.
    /// </summary>
    public void ResetCurrentInput()
    {
        for (int i = 0; i < currentInput.Length; i++)
            currentInput[i] = -1;

        // 만약 이전에 이미 문을 열 만하게 맞췄었다면, 틀린 상태로 바뀌었으니 이벤트 발생
        if (isMatched)
        {
            isMatched = false;
            OnPasswordUnmatched?.Invoke();
        }
    }
}