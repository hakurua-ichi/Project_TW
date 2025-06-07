using UnityEngine;
using TMPro;

public class StateText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText; // TextMeshProUGUI 컴포넌트
    [SerializeField] private float displayDuration = 2f; // 상태 표시 지속 시간
    private bool textVisible; // 상태 표시 텍스트 표시 여부
    private float textTimer = 0f; // 상태 표시 타이머

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textVisible = false;
        statusText.gameObject.SetActive(false); // 상태 텍스트를 비활성화
    }

    // Update is called once per frame
    void Update()
    {
        if(!textVisible)
        {
            statusText.gameObject.SetActive(false); // 상태 텍스트를 비활성화
            return;
        }
        else
        {
            statusText.gameObject.SetActive(true); // 상태 텍스트를 활성화
            textTimer += Time.deltaTime; // 타이머 업데이트
            if (textTimer >= displayDuration) // 2초 후에 텍스트 숨김
            {
                textVisible = false;
                statusText.text = ""; // 상태 표시 텍스트 초기화
                textTimer = 0f; // 타이머 초기화
            }
        }
    }

    public void SetText(bool Visible, string text)
    {
        textVisible = Visible; // 상태 텍스트 표시 여부 설정
        statusText.text = text; // 상태 텍스트 설정

        if(Visible)
        {
            textTimer = 0f;
            statusText.gameObject.SetActive(true);
        }
        else
        {
            statusText.gameObject.SetActive(false);
            textTimer = 0f;
        }
    }

    public void UnVisible()
    {
        textVisible = false; // 상태 텍스트 표시 여부를 false로 설정
        statusText.text = ""; // 상태 표시 텍스트 초기화
        textTimer = 0f; // 타이머 초기화
        statusText.gameObject.SetActive(false); // 상태 텍스트를 비활성화
    }
}
