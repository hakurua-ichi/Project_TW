using UnityEngine;
using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Google; // Google Sign-In 네임스페이스
using Firebase;

public class GoogleAuthHandler : MonoBehaviour
{
    [Header("Google Sign-In Settings")]
    [Tooltip("Firebase 콘솔에서 가져온 웹 클라이언트 ID")]
    public string webClientId = "<YOUR_WEB_CLIENT_ID>"; // Inspector에서 설정!

    private FirebaseAuth _auth;
    private GoogleSignInConfiguration _googleConfiguration;

    private bool _isGoogleSignInInitialized = false;

    // 이벤트 정의
    /// <summary>
    /// Google 인증 시도 완료 시 발생 (성공 여부, 메시지, 사용자 정보)
    /// </summary>
    public event Action<bool, string, FirebaseUser> OnAuthOperationCompleted;


    void Start()
    {
        if (string.IsNullOrEmpty(webClientId) || webClientId == "<YOUR_WEB_CLIENT_ID>")
        {
            Debug.LogError("GoogleAuthHandler: Web Client ID is not set in the Inspector!");
            // OnAuthOperationCompleted?.Invoke(false, "Google Web Client ID가 설정되지 않았습니다.", null); // UI 매니저에게 알릴 수도 있음
            return;
        }

        _googleConfiguration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true,
            RequestEmail = true // Firebase Auth는 ID 토큰만 필요하지만, 사용자 경험을 위해 요청
        };
        _isGoogleSignInInitialized = true; // Google SDK 자체 설정은 여기서 완료

        // FirebaseAuthenticator에서 FirebaseAuth 인스턴스 가져오기
        if (FirebaseAuthenticator.Instance != null && FirebaseAuthenticator.Instance.IsInitialized)
        {
            _auth = FirebaseAuthenticator.Instance.GetFirebaseAuth();
        }
        else if (FirebaseAuthenticator.Instance != null)
        {
            FirebaseAuthenticator.Instance.OnInitializationComplete += HandleFirebaseInitialization;
        }
        else
        {
            Debug.LogError("GoogleAuthHandler: FirebaseAuthenticator instance not found.");
        }
    }

    private void HandleFirebaseInitialization(bool success, string message)
    {
        if (success && FirebaseAuthenticator.Instance != null)
        {
            _auth = FirebaseAuthenticator.Instance.GetFirebaseAuth();
            FirebaseAuthenticator.Instance.OnInitializationComplete -= HandleFirebaseInitialization;
        }
        else
        {
            Debug.LogError($"GoogleAuthHandler: Firebase initialization failed. Cannot get FirebaseAuth. Message: {message}");
        }
    }

    /// <summary>
    /// Google 계정으로 로그인하고 Firebase에 인증합니다.
    /// </summary>
    public async Task SignInWithGoogleAsync()
    {
        if (!_isGoogleSignInInitialized)
        {
            OnAuthOperationCompleted?.Invoke(false, "Google 로그인이 초기화되지 않았습니다. Web Client ID를 확인하세요.", null);
            return;
        }
        if (_auth == null)
        {
            OnAuthOperationCompleted?.Invoke(false, "Firebase 인증이 초기화되지 않았습니다.", null);
            return;
        }

        GoogleSignIn.Configuration = _googleConfiguration;
        try
        {
            GoogleSignInUser googleUser = await GoogleSignIn.DefaultInstance.SignIn();
            if (googleUser == null || string.IsNullOrEmpty(googleUser.IdToken))
            {
                Debug.Log("GoogleAuthHandler: Google Sign-In was cancelled by the user or failed to get ID token.");
                OnAuthOperationCompleted?.Invoke(false, "Google 로그인이 취소되었거나 실패했습니다.", null);
                return;
            }

            Debug.Log($"GoogleAuthHandler: Google Sign-In successful. ID Token: {googleUser.IdToken.Substring(0, 15)}...");

            Credential credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, null);
            // AuthResult authResult = await _auth.SignInWithCredentialAsync(credential); 버그터짐 살려줘
            // FirebaseUser signedInUser = authResult.User;
            FirebaseUser signedInUser = await _auth.SignInWithCredentialAsync(credential);

            Debug.Log($"GoogleAuthHandler: Firebase Sign-In with Google successful: {signedInUser.UserId} ({signedInUser.DisplayName})");
            OnAuthOperationCompleted?.Invoke(true, "Google 로그인 성공!", signedInUser);
        }
        catch (GoogleSignIn.SignInException e) // Google 로그인 자체의 예외
        {
             Debug.LogError($"GoogleAuthHandler: Google Sign-In Exception: Status({e.Status}), Message({e.Message})");
             OnAuthOperationCompleted?.Invoke(false, $"Google 로그인 오류: {e.Status}", null);
        }
        catch (Exception e) // Firebase 인증 또는 기타 예외
        {
            string errorMessage = GetFirebaseAuthErrorMessage(e); // Firebase 예외 메시지 파싱
            Debug.LogError($"GoogleAuthHandler: Google Sign-In or Firebase Auth failed: {errorMessage}");
            OnAuthOperationCompleted?.Invoke(false, $"인증 실패: {errorMessage}", null);
        }
    }

    /// <summary>
    /// 현재 사용자가 Google로 로그인했다면 Google에서도 로그아웃합니다.
    /// FirebaseAuthenticator의 SignOut()이 호출되기 전에 또는 후에 호출될 수 있습니다.
    /// </summary>
    public void SignOutFromGoogle()
    {
        if (_isGoogleSignInInitialized && FirebaseAuthenticator.Instance?.CurrentUser != null)
        {
            // 현재 사용자가 Google 제공자인지 확인 (선택적이지만, 다른 제공자로 로그인한 경우 불필요한 호출 방지)
            bool isGoogleUser = false;
            foreach (var providerData in FirebaseAuthenticator.Instance.CurrentUser.ProviderData)
            {
                if (providerData.ProviderId == GoogleAuthProvider.ProviderId)
                {
                    isGoogleUser = true;
                    break;
                }
            }

            if (isGoogleUser)
            {
                GoogleSignIn.DefaultInstance.SignOut();
                Debug.Log("GoogleAuthHandler: Signed out from Google.");
            }
        }
    }
    
    private string GetFirebaseAuthErrorMessage(Exception e)
    {
        if (e.GetBaseException() is FirebaseException firebaseEx)
        {
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            // 여기에 자주 발생하는 오류 코드에 대한 한글 메시지 매핑 추가 가능
             switch (errorCode)
            {
                // Google 연동 시 발생할 수 있는 특정 오류들
                case AuthError.AccountExistsWithDifferentCredentials:
                    return "이미 다른 방식으로 가입된 계정입니다. 다른 로그인 방식을 시도해보세요.";
                default:
                    return $"오류 코드: {errorCode}";
            }
        }
        return e.Message;
    }

    void OnDestroy()
    {
        if (FirebaseAuthenticator.Instance != null)
        {
            FirebaseAuthenticator.Instance.OnInitializationComplete -= HandleFirebaseInitialization;
        }
    }
}