using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ReadyButton : MonoBehaviour
{
    private Image buttonImage; // 버튼 이미지 컴포넌트
    private Color color1 = new Color(255f / 255f, 174f / 255f, 174f / 255f, 0.5f); // 초기 색상 (연한 빨강)
    private Color color2 = new Color(196f / 255f, 233f / 255f, 255f / 255f, 0.5f); // 변경할 색상 (연한 파랑)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //버튼의 이미지 컴포넌트 가져오기
        buttonImage = GetComponent<Image>();

        //초기 버튼 색깔 설정
        buttonImage.color = color1;

        //버튼 컴포넌트를 가져와 클릭 이벤트에 함수 추가
        GetComponent<Button>().onClick.AddListener(ChangeColor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ChangeColor()
    {
        //버튼 색깔 변경
        if (buttonImage.color.Equals(color1))
            buttonImage.color = color2;
        else if (buttonImage.color.Equals(color2))
            buttonImage.color = color1;

    }
}
