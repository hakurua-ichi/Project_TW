using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionPanelManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button logoutButton;

    [Header("Scene Navigation")]
    [SerializeField] private string loginSceneName = "LoginScene"; // 실제 로그인 씬 이름으로 변경

    void Start()
    {
        if (logoutButton != null)
        {
            logoutButton.onClick.RemoveAllListeners(); // 중복 방지
            logoutButton.onClick.AddListener(HandleLogoutClicked);
        }
        else
        {
            Debug.LogError("OptionPanelManager: Logout Button is not assigned!");
        }

        // 이 패널이 활성화될 때는 사용자가 로그인 상태라고 가정
        if (logoutButton != null)
        {
            logoutButton.interactable = (FirebaseAuthenticator.Instance != null && FirebaseAuthenticator.Instance.CurrentUser != null);
        }


        // FirebaseAuthenticator의 로그아웃 이벤트 구독
        if (FirebaseAuthenticator.Instance != null)
        {
            FirebaseAuthenticator.Instance.OnUserSignedOut += NavigateToLoginSceneOnSignOut;
        }
    }

    void OnDestroy()
    {
        if (FirebaseAuthenticator.Instance != null)
        {
            FirebaseAuthenticator.Instance.OnUserSignedOut -= NavigateToLoginSceneOnSignOut;
        }
    }

    private void HandleLogoutClicked()
    {
        if (FirebaseAuthenticator.Instance != null && FirebaseAuthenticator.Instance.CurrentUser != null)
        {
            Debug.Log("Logout button clicked. Attempting to sign out via OptionPanelManager...");
            FirebaseAuthenticator.Instance.SignOut();
            // SignOut() 호출 후 OnUserSignedOut 이벤트가 발생하여 NavigateToLoginSceneOnSignOut이 호출됨
        }
        else
        {
            Debug.LogWarning("OptionPanelManager: No user to sign out or FirebaseAuthenticator not ready. Navigating to login scene directly.");
            NavigateToLoginScene(); // 혹시 모를 경우 대비
        }
    }

    private void NavigateToLoginSceneOnSignOut()
    {
        // 이 콜백은 FirebaseAuthenticator에서 로그아웃이 "완료된 후" 호출됨
        NavigateToLoginScene();
    }
    
    private void NavigateToLoginScene()
    {
        if (this == null || !gameObject.activeInHierarchy) return; // 이미 파괴되었거나 비활성화된 경우

        Debug.Log("Navigating to login scene from OptionPanelManager.");
        HidePanel(); // 패널을 먼저 숨김
        SceneManager.LoadScene(loginSceneName);
    }


    public void ShowPanel()
    {
        gameObject.SetActive(true);
        if (logoutButton != null && FirebaseAuthenticator.Instance != null) // 패널 표시 시 버튼 상태 갱신
        {
            logoutButton.interactable = (FirebaseAuthenticator.Instance.CurrentUser != null);
        }
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
}