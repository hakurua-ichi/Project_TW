using UnityEngine;
using UnityEngine.UI; // 일반 Button 용
using TMPro;       // TextMeshPro 용
using Firebase.Auth;
using Firebase;
using UnityEngine.SceneManagement;

public class LoginUIManager : MonoBehaviour
{
    [Header("UI Elements - Login/Sign Up")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button signUpButton;
    public Button emailSignInButton;
    public Button logoutButton;
    public TMP_Text statusText; // 상태 메시지 표시용

    [Header("Dependencies")]
    public EmailPasswordAuthHandler emailAuthHandler;

    private bool _isProcessing = false; // 중복 요청 방지 플래그

    void Start()
    {
        // _isProcessing 상태 초기화 (중요!)
        _isProcessing = false;

        // 핸들러 연결 확인
        if (emailAuthHandler == null)
        {
            emailAuthHandler = FindFirstObjectByType<EmailPasswordAuthHandler>();
            if (emailAuthHandler == null) 
                Debug.LogError("LoginUIManager: EmailPasswordAuthHandler를 찾을 수 없습니다!");
        }

        // 먼저 기존 이벤트 구독 해제 (안전하게)
        UnsubscribeFromAuthEvents();
        
        // 버튼 리스너 초기화 - 중복 등록 방지
        signUpButton.onClick.RemoveAllListeners();
        emailSignInButton.onClick.RemoveAllListeners();
        logoutButton.onClick.RemoveAllListeners();
        
        // 버튼 리스너 등록
        signUpButton.onClick.AddListener(HandleSignUpClicked);
        emailSignInButton.onClick.AddListener(HandleEmailSignInClicked);
        logoutButton.onClick.AddListener(HandleLogoutClicked);

        // FirebaseAuthenticator 및 각 핸들러의 이벤트 구독
        SubscribeToAuthEvents();

        // Firebase 초기화 상태 확인
        if (FirebaseAuthenticator.Instance == null)
        {
            SetStatus("Firebase 인스턴스를 찾을 수 없습니다.", true);
            SetAllButtonsInteractable(false);
            return;
        }
        
        if (!FirebaseAuthenticator.Instance.IsInitialized)
        {
            SetStatus("Firebase 초기화 중...", false);
            SetAllButtonsInteractable(false);
        }
        else
        {
            // Firebase가 이미 초기화된 경우, 수동으로 현재 인증 상태 업데이트
            SetStatus("로그인 준비 완료.", false);
            UpdateUIForAuthState(FirebaseAuthenticator.Instance.CurrentUser);
        }
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
        Debug.Log("Unsubscribing from Firebase events.");
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
        SceneManager.LoadScene("MainMenuScene"); // 다음 씬으로 이동 (씬 이름은 필요에 따라 변경)
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

        _isProcessing = true;
        SetAllButtonsInteractable(false);
        SetStatus("로그아웃 중...", false);

        try
        {
            // Firebase 로그아웃
            FirebaseAuthenticator.Instance.SignOut();
            
            // 즉시 UI 업데이트 (이벤트가 제대로 발생하지 않을 경우를 대비)
            UpdateUIForAuthState(null);
            _isProcessing = false;
            SetStatus("로그아웃되었습니다. 다시 로그인해주세요.", false);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"로그아웃 오류: {ex.Message}");
            SetStatus("로그아웃 실패. 나중에 다시 시도해주세요.", true);
            _isProcessing = false;
            SetAllButtonsInteractable(true);
        }
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
            
            // 로그인 입력 필드가 비활성화되어 있다면 활성화
            if (!emailInput.interactable)
            {
                emailInput.interactable = true;
                passwordInput.interactable = true;
                signUpButton.interactable = true;
                emailSignInButton.interactable = true;
            }
            
            // 로그아웃 후 상태 메시지 설정
            if (string.IsNullOrEmpty(statusText.text) || statusText.text.Contains("환영합니다") || statusText.text.Contains("로그아웃"))
            {
                SetStatus("로그인해주세요.", false);
            }
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