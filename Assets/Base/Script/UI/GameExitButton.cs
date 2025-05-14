using System;
using UnityEngine;
using UnityEngine.UI;

public class GameExitButton : MonoBehaviour
{
    [SerializeField] private Button button;

    void Start()
    {
        // 시작 시 버튼에 OnClickExitGame 메소드를 연결
        if(button != null)
        {
            button.onClick.AddListener(OnClickExitGame);
        }
    }

    // 게임 종료 메소드
    public void OnClickExitGame()
    {
        Debug.Log("로그인 창으로 이동");
        FirebaseAuthenticator.Instance.SignOut(); // Firebase 로그아웃
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginUiScene");
    }
}