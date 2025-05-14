using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnStartClicked()
    {
        SceneManager.LoadScene("StageSelectScene"); // 스테이지 선택 화면으로 이동
    }

    public void OnOptionClicked()
    {
        // 옵션 UI 띄우기 or 옵션 씬 로드
        Debug.Log("옵션 클릭됨");
    }

    public void OnQuitClicked()
    {
        Application.Quit();
        Debug.Log("게임 종료"); // 에디터에서는 종료되지 않지만, 빌드 시 종료됨
    }
}
