using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;
using Firebase;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class LoginUIManager : MonoBehaviour
{
    [Header("UI Elements - Login/Sign Up")]
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button continueButton;
    [SerializeField] private TMP_Text statusText;

    [Header("Scene Navigation")]
    [Tooltip("로그인 성공 후 이동할 씬의 이름입니다.")]
    [SerializeField] private string nextSceneName = "MainMenuScene";

    [Header("Dependencies")]
    private EmailPasswordAuthHandler emailAuthHandler;

    private bool _isProcessing = false;
    private TabFocus tabFocus;
    private bool _initialCheckDone = false;

    void Awake()
{
    tabFocus = FindFirstObjectByType<TabFocus>(); // TabFocus는 다른 오브젝트에 있을 수 있으므로 유지
    emailAuthHandler = GetComponent<EmailPasswordAuthHandler>();
    if (emailAuthHandler == null)
    {
        Debug.LogError("LoginUIManager: EmailPasswordAuthHandler 컴포넌트를 이 게임 오브젝트에서 찾을 수 없습니다!");
        if (continueButton != null) continueButton.interactable = false; // Start 이전에 호출되므로 continueButton이 아직 null일 수 있음
        enabled = false; // 스크립트 비활성화
        return;
    }
    }

    void Start()
    {
        if (emailAuthHandler == null) // Awake에서 못 찾았을 경우 대비 (사실상 Awake에서 return 했으면 여기까지 안 옴)
        {
            Debug.LogError("LoginUIManager (Start): EmailPasswordAuthHandler가 없습니다! 로그인 기능이 작동하지 않습니다.");
            if (continueButton != null) continueButton.interactable = false;
            enabled = false;
            return;
        }


        if (tabFocus != null)
        {
            tabFocus.Initialize(emailInput, passwordInput, continueButton);
        }
        else
        {
            Debug.LogWarning("LoginUIManager: TabFocus 스크립트를 찾을 수 없습니다.");
        }

        _isProcessing = false;
        _initialCheckDone = false;

        if (continueButton != null) // Null 체크 후 리스너 등록
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(HandleContinueClicked);
        }

        SubscribeToAuthEvents(); // 이벤트 구독

        if (FirebaseAuthenticator.Instance == null)
        {
            SetStatus("Firebase 인스턴스를 찾을 수 없습니다. (Start)", true);
            if (continueButton != null) continueButton.interactable = false;
            return;
        }

        if (!FirebaseAuthenticator.Instance.IsInitialized)
        {
            SetStatus("Firebase 초기화 중... (Start)", false);
            if (continueButton != null) continueButton.interactable = false;
        }
    }

    bool ValidateUIElements()
    {
        if (emailInput == null || passwordInput == null || continueButton == null || statusText == null)
        {
            Debug.LogError("LoginUIManager: 하나 이상의 UI 요소가 인스펙터에 연결되지 않았습니다! 스크립트를 비활성화합니다.");
            if (this.enabled) this.enabled = false; // 자기 자신 비활성화
            if (continueButton != null) continueButton.interactable = false;
            return false;
        }
        return true;
    }


    void OnDestroy()
    {
        UnsubscribeFromAuthEvents();
    }

    private void SubscribeToAuthEvents()
    {
        Debug.Log("DEBUG: SubscribeToAuthEvents CALLED.");
        if (FirebaseAuthenticator.Instance != null)
        {
            FirebaseAuthenticator.Instance.OnInitializationComplete += HandleFirebaseInitialization;
            FirebaseAuthenticator.Instance.OnUserSignedIn += HandleUserSignedIn;
            FirebaseAuthenticator.Instance.OnUserSignedOut += HandleUserSignedOut;
        }
        else
        {
            Debug.LogError("DEBUG: FirebaseAuthenticator.Instance IS NULL in SubscribeToAuthEvents! Cannot subscribe.");
        }

        if (emailAuthHandler != null)
        {
            emailAuthHandler.OnAuthOperationCompleted += HandleAuthOperationCompleted;
        }
        else
        {
            Debug.LogWarning("DEBUG: emailAuthHandler is NULL in SubscribeToAuthEvents.");
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
    // LoginUIManager.cs
    private void HandleFirebaseInitialization(bool success, string message)
    {
        if (this == null || !gameObject.activeInHierarchy) return;

        string currentUserId = (FirebaseAuthenticator.Instance?.CurrentUser == null) ? "NULL" : FirebaseAuthenticator.Instance.CurrentUser.UserId;

        if (success)
        {
            if (!_initialCheckDone && FirebaseAuthenticator.Instance != null && FirebaseAuthenticator.Instance.CurrentUser != null)
            {
                _initialCheckDone = true;
                Debug.Log("User already signed in (HandleFirebaseInitialization). Navigating to next scene.");
                SceneManager.LoadScene(nextSceneName);
                return;
            }
            
            SetStatus("이메일과 비밀번호를 입력하세요.", false);
            if (continueButton != null && FirebaseAuthenticator.Instance != null)
            {
                bool shouldBeInteractable = (FirebaseAuthenticator.Instance.CurrentUser == null && !_isProcessing);
                Debug.Log($"DEBUG: Setting continueButton.interactable to {shouldBeInteractable} (CurrentUser is {(FirebaseAuthenticator.Instance.CurrentUser == null ? "NULL" : "NOT NULL")}, _isProcessing is {_isProcessing})");
                continueButton.interactable = shouldBeInteractable;
            }
        }
        else
        {
            SetStatus($"초기화 오류: {message}", true);
            if (continueButton != null) continueButton.interactable = false;
        }
        _initialCheckDone = true;
    }

    private void HandleUserSignedIn(FirebaseUser user)
    {
        if (this == null || !gameObject.activeInHierarchy) return;

        _isProcessing = false; // 로그인 성공 시 처리 중 상태 해제
        // 버튼 상태는 UpdateUIForAuthState에서 관리 또는 여기서 명시적 업데이트
        if(continueButton != null) continueButton.interactable = false; // 로그인 되었으므로 비활성화

        SetStatus($"환영합니다, {user.DisplayName ?? user.Email}!", false);

        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("LoginUIManager: nextSceneName이 설정되지 않았습니다! 씬을 이동할 수 없습니다.");
            return;
        }
        SceneManager.LoadScene(nextSceneName);
        Debug.Log($"Login Succeeded. Navigating to scene: {nextSceneName}");
    }

    private void HandleUserSignedOut()
    {
        if (this == null || !gameObject.activeInHierarchy) return;
        _initialCheckDone = false;
        Debug.Log("LoginUIManager: HandleUserSignedOut called, _initialCheckDone set to false.");
        SetStatus("로그아웃되었습니다. 다시 로그인해주세요.", false);
        _isProcessing = false;
        UpdateUIForAuthState(null);
    }

    private void HandleAuthOperationCompleted(bool success, string message, FirebaseUser user)
    {
        if (this == null || !gameObject.activeInHierarchy) return;

        _isProcessing = false;
        SetStatus(message, !success);

        if (success)
        {
            // 성공 시 UI 정리는 HandleUserSignedIn에서 처리
            if (emailInput != null) emailInput.text = "";
            if (passwordInput != null) passwordInput.text = "";
            // 버튼 상태는 HandleUserSignedIn에서 로그인 후 비활성화됨
        }
        else // 실패 시
        {
            if (continueButton != null) continueButton.interactable = true;
        }
    }
    #endregion

    #region Button Click Handlers
    private async void HandleContinueClicked()
    {
        if (emailAuthHandler == null)
        {
            SetStatus("인증 핸들러가 준비되지 않았습니다. (null)", true);
            Debug.LogError("EmailAuthHandler is null in HandleContinueClicked.");
            return;
        }
        if (FirebaseAuthenticator.Instance == null || !FirebaseAuthenticator.Instance.IsInitialized)
        {
            SetStatus("Firebase가 준비되지 않았습니다. (not init)", true);
            return;
        }
        if (_isProcessing) return;
        if (!ValidateInputs()) return;

        _isProcessing = true;
        if (continueButton != null) continueButton.interactable = false;
        SetStatus("계정 정보를 확인 중입니다...", false);

        await emailAuthHandler.AttemptSignUpOrSignInAsync(emailInput.text, passwordInput.text);
    }
    #endregion

    #region UI Helper Methods
    private bool ValidateInputs()
    {
        if (emailInput == null || passwordInput == null) return false; // Defensive check

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
        if (this == null || !gameObject.activeInHierarchy) return;
        if (!ValidateUIElements()) return; // UI 요소가 없으면 업데이트 불가

        bool isSignedIn = user != null;

        emailInput.gameObject.SetActive(!isSignedIn);
        passwordInput.gameObject.SetActive(!isSignedIn);
        continueButton.gameObject.SetActive(!isSignedIn);

        bool canInteract = !isSignedIn && !_isProcessing;
        emailInput.interactable = canInteract;
        passwordInput.interactable = canInteract;
        continueButton.interactable = canInteract;

        if (!isSignedIn)
        {
            emailInput.text = "";
            passwordInput.text = "";
            if (FirebaseAuthenticator.Instance != null && FirebaseAuthenticator.Instance.IsInitialized && statusText != null)
            {
                if (string.IsNullOrEmpty(statusText.text) || statusText.text.Contains("환영합니다") || statusText.text.Contains("로그아웃"))
                {
                    SetStatus("이메일과 비밀번호를 입력하세요.", false);
                }
            }
        }
    }

    private void SetStatus(string message, bool isError)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = isError ? Color.red : new Color(0.2f, 0.2f, 0.2f);
        }
        if (isError) Debug.LogError($"LoginUIManager Status: {message}");
        else Debug.Log($"LoginUIManager Status: {message}");
    }
    #endregion
}