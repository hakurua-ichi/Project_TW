using System;
using UnityEngine;
using UnityEngine.UI;

public class AuthUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public InputField emailInput;
    public InputField passwordInput;
    public Button loginRegisterButton;
    public Toggle autoLoginToggle;
    public Text statusText;
    public Button logoutButton; // 이 버튼은 다른 씬에 있을 수 있으므로, 해당 씬의 UI 매니저가 관리할 수 있음

    // 이벤트: 로그인/회원가입 버튼 클릭 시 호출 (이메일, 비밀번호 전달)
    public event Action<string, string> OnLoginRegisterAttempt;
    // 이벤트: 로그아웃 버튼 클릭 시 호출
    public event Action OnLogoutAttempt;
    // 이벤트: 자동 로그인 토글 변경 시 호출 (토글 상태 전달)
    public event Action<bool> OnAutoLoginToggleChanged;

    void Start()
    {
        if (loginRegisterButton != null)
            loginRegisterButton.onClick.AddListener(HandleLoginRegisterClick);
        if (logoutButton != null)
            logoutButton.onClick.AddListener(HandleLogoutClick);
        if (autoLoginToggle != null)
            autoLoginToggle.onValueChanged.AddListener(HandleAutoLoginToggle);
    }

    private void HandleLoginRegisterClick()
    {
        OnLoginRegisterAttempt?.Invoke(emailInput.text, passwordInput.text);
    }

    private void HandleLogoutClick()
    {
        OnLogoutAttempt?.Invoke();
    }

    private void HandleAutoLoginToggle(bool isOn)
    {
        OnAutoLoginToggleChanged?.Invoke(isOn);
    }

    public string GetEmail() => emailInput.text;
    public string GetPassword() => passwordInput.text;
    public bool IsAutoLoginEnabled() => autoLoginToggle != null && autoLoginToggle.isOn;

    public void SetAutoLoginToggleState(bool isOn)
    {
        if (autoLoginToggle != null)
        {
            autoLoginToggle.isOn = isOn;
        }
    }

    public void DisplayStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        Debug.Log($"UI Status: {message}");
    }

    public void ClearInputs()
    {
        if (emailInput != null) emailInput.text = "";
        if (passwordInput != null) passwordInput.text = "";
    }

    public void SetLoginButtonInteractable(bool interactable)
    {
        if(loginRegisterButton != null) loginRegisterButton.interactable = interactable;
    }
}