using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartSceneManager : MonoBehaviour
{
    public void LogIn()
    {
        SceneManager.LoadScene("LogInScene"); // 스테이지 선택 화면으로 이동
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("게임 종료"); // 에디터에서는 종료되지 않지만, 빌드 시 종료됨
    }

    public void LogOut()
    {
        SceneManager.LoadScene("LoginUiScene"); // 로그인 화면으로 이동
    }
}
