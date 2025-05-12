using Firebase.Auth;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthController : MonoBehaviour
{
    [Header("Dependencies")]
    public FirebaseAuthenticator authenticator; // Inspector에서 연결
    public AuthUIManager uiManager;             // Inspector에서 연결

    [Header("Configuration")]
    public string nextSceneName = "HomeScene";
    public int minPasswordLength = 8;
    // public string passwordRegexPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";

    private const string AutoLoginPrefKey = "AutoLoginEnabled";

    void Start()
    {
        // 의존성 확인
        if (authenticator == null)
        {
            Debug.LogError("AuthController: FirebaseAuthenticator is not assigned!");
            enabled = false; // 스크립트 비활성화
            return;
        }
        if (uiManager == null)
        {
            Debug.LogError("AuthController: AuthUIManager is not assigned!");
            enabled = false;
            return;
        }

        // 이벤트 구독
        authenticator.OnInitializationCompleted += HandleFirebaseInitializationCompleted;
        authenticator.OnAuthStateChanged += HandleAuthStateChanged;
        uiManager.OnLoginRegisterAttempt += HandleLoginRegisterRequest;
        uiManager.OnLogoutAttempt += HandleLogoutRequest; // uiManager에 로그아웃 버튼이 있다면
        uiManager.OnAutoLoginToggleChanged += HandleAutoLoginToggleChange;

        // 자동 로그인 토글 초기 상태 설정
        uiManager.SetAutoLoginToggleState(PlayerPrefs.GetInt(AutoLoginPrefKey, 0) == 1);

        // Firebase 초기화 시작
        if (!authenticator.IsInitialized)
        {
            uiManager.DisplayStatus("Firebase 초기화 중...");
            uiManager.SetLoginButtonInteractable(false);
            authenticator.Initialize();
        }
        else // 이미 초기화 되었다면 (예: DontDestroyOnLoad 사용 시)
        {
             HandleFirebaseInitializationCompleted(true, "Firebase 이미 초기화됨");
        }
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (authenticator != null)
        {
            authenticator.OnInitializationCompleted -= HandleFirebaseInitializationCompleted;
            authenticator.OnAuthStateChanged -= HandleAuthStateChanged;
        }
        if (uiManager != null)
        {
            uiManager.OnLoginRegisterAttempt -= HandleLoginRegisterRequest;
            uiManager.OnLogoutAttempt -= HandleLogoutRequest;
            uiManager.OnAutoLoginToggleChanged -= HandleAutoLoginToggleChange;
        }
    }

    private void HandleFirebaseInitializationCompleted(bool success, string message)
    {
        uiManager.DisplayStatus(message);
        uiManager.SetLoginButtonInteractable(success);

        if (success && PlayerPrefs.GetInt(AutoLoginPrefKey, 0) == 1 && authenticator.CurrentUser != null)
        {
            // Firebase 초기화 직후, 자동 로그인이 켜져 있고, 이미 로그인된 사용자가 있다면 (앱 재시작 시)
            Debug.Log("자동 로그인 조건 충족 (초기화 시). 다음 씬으로 이동 시도.");
            LoadNextScene();
        }
        else if (success && authenticator.CurrentUser == null)
        {
            uiManager.DisplayStatus("로그인 또는 회원가입을 진행해주세요.");
        }
    }

    private void HandleAuthStateChanged(FirebaseUser user)
    {
        if (user != null) // 로그인 성공
        {
            uiManager.DisplayStatus($"로그인 성공: {user.Email}");
            Debug.Log($"AuthController: User signed in - {user.Email}");
            // 자동 로그인이 활성화되어 있거나, 방금 수동으로 로그인/가입한 경우 다음 씬으로
            if (PlayerPrefs.GetInt(AutoLoginPrefKey, 0) == 1 || SceneManager.GetActiveScene().name != nextSceneName)
            {
                LoadNextScene();
            }
        }
        else // 로그아웃 성공 또는 초기 상태
        {
            uiManager.DisplayStatus("로그아웃 되었거나 로그인되지 않았습니다.");
            Debug.Log("AuthController: User signed out or not logged in.");
            // 현재 씬이 nextSceneName(예: HomeScene)이고 로그아웃 되었다면 로그인 씬으로 이동
            if (SceneManager.GetActiveScene().name == nextSceneName)
            {
                LoadLoginScene();
            }
        }
    }

    private async void HandleLoginRegisterRequest(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            uiManager.DisplayStatus("이메일과 비밀번호를 모두 입력해주세요.");
            return;
        }

        uiManager.DisplayStatus("로그인 시도 중...");
        uiManager.SetLoginButtonInteractable(false);

        var (signedInUser, signInError, signInMsg) = await authenticator.SignInUserAsync(email, password);

        if (signedInUser != null)
        {
            // 성공 시 AuthStateChanged가 처리
            // uiManager.DisplayStatus($"로그인 성공: {signedInUser.Email}");
            // LoadNextScene(); // AuthStateChanged에서 처리
        }
        else
        {
            if (signInError == AuthError.UserNotFound)
            {
                uiManager.DisplayStatus("계정이 없습니다. 회원가입을 시도합니다...");
                if (!IsPasswordValid(password, out string passwordErrorMsg))
                {
                    uiManager.DisplayStatus(passwordErrorMsg);
                    uiManager.SetLoginButtonInteractable(true);
                    return;
                }
                var (createdUser, createError, createMsg) = await authenticator.CreateUserAsync(email, password);
                if (createdUser != null)
                {
                    // 성공 시 AuthStateChanged가 처리
                    // uiManager.DisplayStatus($"회원가입 성공: {createdUser.Email}");
                    // LoadNextScene(); // AuthStateChanged에서 처리
                }
                else
                {
                    uiManager.DisplayStatus($"회원가입 실패: {TranslateAuthError(createError, createMsg)}");
                }
            }
            else
            {
                uiManager.DisplayStatus($"로그인 실패: {TranslateAuthError(signInError, signInMsg)}");
            }
        }
        uiManager.SetLoginButtonInteractable(true);
    }

    public void HandleLogoutRequest() // 외부(예: HomeScene의 UI 매니저)에서도 호출 가능하도록 public
    {
        if (authenticator.CurrentUser != null)
        {
            uiManager.DisplayStatus("로그아웃 중...");
            authenticator.SignOut();
            PlayerPrefs.SetInt(AutoLoginPrefKey, 0); // 로그아웃 시 자동 로그인 비활성화
            PlayerPrefs.Save();
            // AuthStateChanged가 로그인 씬으로의 전환을 처리할 것임
        }
        else
        {
            uiManager.DisplayStatus("이미 로그아웃 상태입니다.");
        }
    }

    private void HandleAutoLoginToggleChange(bool isOn)
    {
        PlayerPrefs.SetInt(AutoLoginPrefKey, isOn ? 1 : 0);
        PlayerPrefs.Save();
        uiManager.DisplayStatus("자동 로그인 설정: " + (isOn ? "활성화" : "비활성화"));
    }

    private bool IsPasswordValid(string password, out string errorMessage)
    {
        errorMessage = "";
        if (string.IsNullOrEmpty(password) || password.Length < minPasswordLength)
        {
            errorMessage = $"비밀번호는 최소 {minPasswordLength}자 이상이어야 합니다.";
            return false;
        }
        // if (!string.IsNullOrEmpty(passwordRegexPattern) && !Regex.IsMatch(password, passwordRegexPattern))
        // {
        //     errorMessage = "비밀번호가 정책에 맞지 않습니다.";
        //     return false;
        // }
        return true;
    }

    private string TranslateAuthError(AuthError error, string defaultMessage)
    {
        switch (error)
        {
            case AuthError.None: return "성공"; // 보통 에러가 아닐 때 호출되진 않음
            case AuthError.Cancelled: return "작업이 취소되었습니다.";
            case AuthError.InvalidEmail: return "유효하지 않은 이메일 형식입니다.";
            case AuthError.WrongPassword: return "잘못된 비밀번호입니다.";
            case AuthError.UserNotFound: return "등록되지 않은 사용자입니다.";
            case AuthError.EmailAlreadyInUse: return "이미 사용 중인 이메일입니다.";
            case AuthError.WeakPassword: return "비밀번호가 너무 약합니다. (Firebase 기준)";
            // 추가적인 에러 코드 번역
            default:
                Debug.LogWarning($"Unhandled AuthError: {error} ({defaultMessage})");
                return defaultMessage ?? "알 수 없는 오류가 발생했습니다.";
        }
    }


    private void LoadNextScene()
    {
        if (SceneManager.GetActiveScene().name != nextSceneName)
        {
            Debug.Log($"Loading scene: {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void LoadLoginScene()
    {
        // LoginScene의 이름이나 빌드 인덱스를 알아야 합니다.
        // 여기서는 LoginScene이 빌드 인덱스 0이거나 이름이 "LoginScene"이라고 가정합니다.
        string loginSceneName = "LoginScene"; // 실제 로그인 씬 이름으로 변경
        if (SceneManager.GetActiveScene().name != loginSceneName)
        {
             for(int i=0; i < SceneManager.sceneCountInBuildSettings; i++)
             {
                 string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                 if(System.IO.Path.GetFileNameWithoutExtension(scenePath) == loginSceneName)
                 {
                     SceneManager.LoadScene(i);
                     return;
                 }
             }
             Debug.LogError($"LoginScene '{loginSceneName}' not found in build settings!");
        }
    }
}