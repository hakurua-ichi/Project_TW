using UnityEngine;
using UnityEngine.UI; // 일반 Button 용
using TMPro;       // TextMeshPro 용
using Firebase.Auth;
using Firebase;
using UnityEngine.SceneManagement;
using System.Reflection;
using System;

public class LoginUIManager : MonoBehaviour
{
    [Header("UI Elements - Login/Sign Up")]
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button signUpButton;
    [SerializeField] private Button emailSignInButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private TMP_Text statusText; // 상태 메시지 표시용

    [Header("Dependencies")]
    [SerializeField] private EmailPasswordAuthHandler emailAuthHandler;

    private bool _isProcessing = false; // 중복 요청 방지 플래그

    void Start()
    {
        

        // 버튼 리스너 등록
        signUpButton.onClick.AddListener(HandleSignUpClicked);
        emailSignInButton.onClick.AddListener(HandleEmailSignInClicked);
        logoutButton.onClick.AddListener(HandleLogoutClicked);

        // FirebaseAuthenticator 및 각 핸들러의 이벤트 구독
        SubscribeToAuthEvents();

        // 초기 UI 상태 설정 (예: Firebase 초기화 중 메시지)
        if (FirebaseAuthenticator.Instance == null || !FirebaseAuthenticator.Instance.IsInitialized)
        {
            SetStatus("Firebase 초기화 중...", false);
        }
        UpdateUIForAuthState(FirebaseAuthenticator.Instance?.CurrentUser);

        // 핸들러 연결 확인
        if (emailAuthHandler == null) Debug.LogError("LoginUIManager: EmailPasswordAuthHandler is not assigned!");
        Debug.Log("이메일 핸들러: " + emailAuthHandler);
    }

    void OnDestroy()
    {
        UnsubscribeFromAuthEvents();
    }

    private void SubscribeToAuthEvents()
    {
        if (FirebaseAuthenticator.Instance != null)
        {
            FirebaseAuthenticator.Instance.OnInitializationComplete += HandleFirebaseInitialization;
            FirebaseAuthenticator.Instance.OnUserSignedIn += HandleUserSignedIn;
            FirebaseAuthenticator.Instance.OnUserSignedOut += HandleUserSignedOut; // 로그인 씬에서는 로그아웃 상태로 UI 업데이트
        }
        if (emailAuthHandler != null)
        {
            emailAuthHandler.OnAuthOperationCompleted += HandleAuthOperationCompleted;
        }
    }

    private void UnsubscribeFromAuthEvents()
    {
        if (FirebaseAuthenticator.Instance != null)
        {
            FirebaseAuthenticator.Instance.OnInitializationComplete -= HandleFirebaseInitialization;
            FirebaseAuthenticator.Instance.OnUserSignedIn -= HandleUserSignedIn;
            FirebaseAuthenticator.Instance.OnUserSignedOut -= HandleUserSignedOut;
        }
        if (emailAuthHandler != null)
        {
            emailAuthHandler.OnAuthOperationCompleted -= HandleAuthOperationCompleted;
        }
    }

    #region Event Handlers

    private void HandleFirebaseInitialization(bool success, string message)
    {
        if (success)
        {
            SetStatus("로그인 준비 완료.", false);
            UpdateUIForAuthState(FirebaseAuthenticator.Instance.CurrentUser);
        }
        else
        {
            SetStatus($"초기화 오류: {message}", true);
            SetAllButtonsInteractable(false); // 초기화 실패 시 버튼 비활성화
        }
    }

    private void HandleUserSignedIn(FirebaseUser user)
    {
        SetStatus($"환영합니다, {user.DisplayName ?? user.Email}!", false);
        _isProcessing = false; // 로그인 성공 시 처리 중 상태 해제

        // TODO: 로그인 성공 후 다음 씬으로 이동
        SceneManager.LoadScene("MainMenuScene");
        Debug.Log("Login Succeeded. Navigating to next scene...");
    }

    private void HandleUserSignedOut()
    {
        SetStatus("로그아웃되었습니다. 다시 로그인해주세요.", false);
        _isProcessing = false; // 로그아웃 시 처리 중 상태 해제
        UpdateUIForAuthState(null); // 로그아웃 상태의 UI로 변경
    }

    private void HandleAuthOperationCompleted(bool success, string message, FirebaseUser user)
    {
        _isProcessing = false; // 작업 완료 후 처리 중 상태 해제
        SetAllButtonsInteractable(true); // 버튼 다시 활성화

        SetStatus(message, !success);

        if (success)
        {
            // 로그인/회원가입 성공 시 HandleUserSignedIn에서 씬 전환 처리됨
            // 여기서는 추가적인 UI 업데이트가 필요하다면 수행 (예: 입력 필드 초기화)
            emailInput.text = "";
            passwordInput.text = "";
        }
        // 실패 시에는 메시지만 표시되고 UI는 현재 상태 유지
    }

    private void HandleLogoutClicked()
    {
        if (_isProcessing || FirebaseAuthenticator.Instance == null) return;

        _isProcessing = true; // 로그아웃 처리 중
        SetAllButtonsInteractable(false); // 모든 인터랙션 비활성화 (로그아웃 버튼 포함)
        SetStatus("로그아웃 중...", false);

        // Firebase 로그아웃
        FirebaseAuthenticator.Instance.SignOut();

        emailAuthHandler = GameObject.Find("AuthService").GetComponent<EmailPasswordAuthHandler>();

        // FirebaseAuthenticator.Instance.SignOut() 호출 후
        // OnUserSignedOut 이벤트가 발생하여 HandleUserSignedOut이 호출되고,
        // 거기서 UI 업데이트 및 _isProcessing = false 처리가 이루어집니다.
        // 따라서 여기서 직접 _isProcessing = false 등을 호출할 필요는 없습니다.
    }

    #endregion

    #region Button Click Handlers

    private async void HandleSignUpClicked()
    {
        if (_isProcessing || emailAuthHandler == null) return;
        if (!ValidateInputs()) return;

        _isProcessing = true;
        SetAllButtonsInteractable(false);
        SetStatus("회원가입 중...", false);
        await emailAuthHandler.SignUpWithEmailAsync(emailInput.text, passwordInput.text);
    }

    private async void HandleEmailSignInClicked()
    {
        if (_isProcessing || emailAuthHandler == null) return;
        if (!ValidateInputs()) return;

        _isProcessing = true;
        SetAllButtonsInteractable(false);
        SetStatus("로그인 중...", false);
        await emailAuthHandler.SignInWithEmailAsync(emailInput.text, passwordInput.text);
    }

    #endregion

    #region UI Helper Methods

    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(emailInput.text))
        {
            SetStatus("이메일을 입력해주세요.", true);
            return false;
        }
        if (string.IsNullOrWhiteSpace(passwordInput.text))
        {
            SetStatus("비밀번호를 입력해주세요.", true);
            return false;
        }
        return true;
    }

    private void UpdateUIForAuthState(FirebaseUser user)
    {
        bool isSignedIn = user != null;

        // 로그인/회원가입 관련 UI
        emailInput.gameObject.SetActive(!isSignedIn);
        passwordInput.gameObject.SetActive(!isSignedIn);
        signUpButton.gameObject.SetActive(!isSignedIn);
        emailSignInButton.gameObject.SetActive(!isSignedIn);

        // 로그아웃 버튼은 로그인 상태일 때만 활성화/보이기
        if (logoutButton != null)
        {
            logoutButton.gameObject.SetActive(isSignedIn);
            logoutButton.interactable = isSignedIn && !_isProcessing; // 로그인 했고, 처리중 아닐때
        }

        // 로그인 안 했거나 처리 중이 아닐 때 다른 버튼들 활성화
        bool canInteractWithAuthButtons = !isSignedIn && !_isProcessing;
        signUpButton.interactable = canInteractWithAuthButtons;
        emailSignInButton.interactable = canInteractWithAuthButtons;
        emailInput.interactable = canInteractWithAuthButtons;
        passwordInput.interactable = canInteractWithAuthButtons;

        if (!isSignedIn)
        {
            emailInput.text = "";
            passwordInput.text = "";
            // 로그인 화면으로 돌아왔을 때 초기 상태 메시지 (선택적)
            // if (statusText.text.Contains("환영합니다")) // 이전 로그인 성공 메시지가 남아있다면
            // {
            //     SetStatus("로그인해주세요.", false);
            // }
        }
    }

    private void SetAllButtonsInteractable(bool interactable)
    {
        // 로그인/가입 관련 버튼들
        signUpButton.interactable = interactable;
        emailSignInButton.interactable = interactable;
        emailInput.interactable = interactable;
        passwordInput.interactable = interactable;

        // 로그아웃 버튼도 제어 (단, 로그아웃 버튼의 활성화/비활성화는 로그인 상태에 따라 더 우선적으로 결정될 수 있음)
        if (logoutButton != null)
        {
            // 로그아웃 버튼은 '로그인 상태' 이면서 '처리중이 아닐 때'만 활성화되어야 함
            // 이 함수는 _isProcessing 상태 변경 시 주로 호출되므로,
            // 현재 로그인 상태를 고려하여 interactable을 설정해야 함.
            bool isSignedIn = FirebaseAuthenticator.Instance?.CurrentUser != null;
            logoutButton.interactable = isSignedIn && interactable;
        }
    }

    private void SetStatus(string message, bool isError)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = isError ? Color.red : new Color(0.2f, 0.2f, 0.2f); // 기본 텍스트 색상 (검은색 계열)
        }
        if (isError) Debug.LogError($"LoginUIManager Status: {message}");
        else Debug.Log($"LoginUIManager Status: {message}");
    }
    #endregion
}