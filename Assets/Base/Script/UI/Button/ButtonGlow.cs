using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonGlow : MonoBehaviour
{
    public Image glowImage;

    void Start()
    {
        // 시작할 때 glowImage를 숨김 상태로 초기화
        if (glowImage != null)
        {
            glowImage.color = new Color(1, 1, 1, 0);
        }
    }

    public void ShowGlow()
    {
        glowImage.color = new Color(1, 1, 1, 1); // 완전 보이게
    }

    public void HideGlow()
    {
        glowImage.color = new Color(1, 1, 1, 0); // 숨김
    }
}