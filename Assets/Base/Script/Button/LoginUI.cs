using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField idInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button googleLoginButton;

    void Start()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
        googleLoginButton.onClick.AddListener(OnGoogleLoginClicked);
    }

    void OnLoginClicked()
    {
        string userId = idInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("ID 또는 비밀번호가 비어있습니다.");
            return;
        }

        // 실제 로그인 처리 (서버 연동 등)
        Debug.Log($"로그인 시도: ID = {userId}, Password = {password}");
    }

    void OnGoogleLoginClicked()
    {
        Debug.Log("Google 로그인 버튼 클릭됨");
        // 여기에 외부 연동 또는 OAuth 처리 로직
    }
}