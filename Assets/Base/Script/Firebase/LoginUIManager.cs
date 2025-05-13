using UnityEngine;
using UnityEngine.UI; // 일반 Button 용
using TMPro;       // TextMeshPro 용
using Firebase.Auth;
using Firebase;

public class LoginUIManager : MonoBehaviour
{
    [Header("UI Elements - Login/Sign Up")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button signUpButton;
    public Button emailSignInButton;
    public Button googleSignInButton;
    public TMP_Text statusText; // 상태 메시지 표시용

    [Header("Dependencies")]
    public EmailPasswordAuthHandler emailAuthHandler;
    public GoogleAuthHandler googleAuthHandler;
    // public SceneNavigationService sceneNavigator; // 씬 네비게이터 (필요 시)

    private bool _isProcessing = false; // 중복 요청 방지 플래그

    void Start()
    {
        // 핸들러 연결 확인
        if (emailAuthHandler == null) Debug.LogError("LoginUIManager: EmailPasswordAuthHandler is not assigned!");
        if (googleAuthHandler == null) Debug.LogError("LoginUIManager: GoogleAuthHandler is not assigned!");
        // if (sceneNavigator == null) Debug.LogError("LoginUIManager: SceneNavigationService is not assigned!");


        // 버튼 리스너 등록
        signUpButton.onClick.AddListener(HandleSignUpClicked);
        emailSignInButton.onClick.AddListener(HandleEmailSignInClicked);
        googleSignInButton.onClick.AddListener(HandleGoogleSignInClicked);

        // FirebaseAuthenticator 및 각 핸들러의 이벤트 구독
        SubscribeToAuthEvents();

        // 초기 UI 상태 설정 (예: Firebase 초기화 중 메시지)
        if (FirebaseAuthenticator.Instance == null || !FirebaseAuthenticator.Instance.IsInitialized)
        {
            SetStatus("Firebase 초기화 중...", false);
        }
        UpdateUIForAuthState(FirebaseAuthenticator.Instance?.CurrentUser);
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
        if (googleAuthHandler != null)
        {
            googleAuthHandler.OnAuthOperationCompleted += HandleAuthOperationCompleted;
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
        if (googleAuthHandler != null)
        {
            googleAuthHandler.OnAuthOperationCompleted -= HandleAuthOperationCompleted;
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

    private async void HandleGoogleSignInClicked()
    {
        if (_isProcessing || googleAuthHandler == null) return;

        _isProcessing = true;
        SetAllButtonsInteractable(false);
        SetStatus("Google 로그인 진행 중...", false);
        await googleAuthHandler.SignInWithGoogleAsync();
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

        // 로그인/회원가입 관련 UI는 로그아웃 상태일 때만 활성화
        emailInput.gameObject.SetActive(!isSignedIn);
        passwordInput.gameObject.SetActive(!isSignedIn);
        signUpButton.gameObject.SetActive(!isSignedIn);
        emailSignInButton.gameObject.SetActive(!isSignedIn);
        googleSignInButton.gameObject.SetActive(!isSignedIn);

        SetAllButtonsInteractable(!isSignedIn && !_isProcessing); // 로그인 안했고, 처리중도 아닐때 활성화

        if (!isSignedIn)
        {
             emailInput.text = "";
             passwordInput.text = "";
        }
    }

    private void SetAllButtonsInteractable(bool interactable)
    {
        signUpButton.interactable = interactable;
        emailSignInButton.interactable = interactable;
        googleSignInButton.interactable = interactable;
        // 입력 필드도 함께 제어
        emailInput.interactable = interactable;
        passwordInput.interactable = interactable;
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