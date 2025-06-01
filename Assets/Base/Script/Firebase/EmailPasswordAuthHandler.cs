using UnityEngine;
using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase;

public class EmailPasswordAuthHandler : MonoBehaviour
{
    private FirebaseAuth _auth;
    public event Action<bool, string, FirebaseUser> OnAuthOperationCompleted;

    void Start()
    {
        if (FirebaseAuthenticator.Instance != null && FirebaseAuthenticator.Instance.IsInitialized)
        {
            InitializeAuth();
        }
        else if (FirebaseAuthenticator.Instance != null)
        {
            FirebaseAuthenticator.Instance.OnInitializationComplete += HandleFirebaseInitForAuthHandler;
        }
        else
        {
            Debug.LogError("EmailPasswordAuthHandler: FirebaseAuthenticator instance not found. Cannot initialize.");
            // OnAuthOperationCompleted?.Invoke(false, "인증 시스템 초기화 실패 (Authenticator 없음)", null); // 필요 시
        }
    }

    private void HandleFirebaseInitForAuthHandler(bool success, string message)
    {
        if (FirebaseAuthenticator.Instance != null)
        {
            FirebaseAuthenticator.Instance.OnInitializationComplete -= HandleFirebaseInitForAuthHandler;
            if (success)
            {
                InitializeAuth();
            }
            else
            {
                Debug.LogError($"EmailPasswordAuthHandler: Firebase initialization failed. Message: {message}");
                // OnAuthOperationCompleted?.Invoke(false, "인증 시스템 초기화 실패 (Firebase 실패)", null); // 필요 시
            }
        }
    }

    private void InitializeAuth()
    {
        _auth = FirebaseAuthenticator.Instance.GetFirebaseAuth();
        if (_auth == null)
        {
            Debug.LogError("EmailPasswordAuthHandler: Failed to get FirebaseAuth instance after initialization.");
            // OnAuthOperationCompleted?.Invoke(false, "인증 시스템 초기화 실패 (Auth 객체 없음)", null); // 필요 시
        }
        else
        {
            Debug.Log("EmailPasswordAuthHandler: FirebaseAuth initialized successfully.");
        }
    }


    public async Task AttemptSignUpOrSignInAsync(string email, string password)
    {
        if (_auth == null)
        {
            Debug.LogError("EmailPasswordAuthHandler: FirebaseAuth is not ready for auth attempt.");
            OnAuthOperationCompleted?.Invoke(false, "인증 시스템이 준비되지 않았습니다. 잠시 후 다시 시도해주세요.", null);
            return;
        }

        try
        {
            Debug.Log($"Attempting to create user: {email}");
            // 회원가입 성공 시, Firebase의 StateChanged 이벤트가 OnUserSignedIn을 호출하여
            // LoginUIManager의 HandleUserSignedIn이 실행될 것입니다.
            // 여기서는 OnAuthOperationCompleted를 호출하여 _isProcessing 해제 등을 돕습니다.
            AuthResult signUpResult = await _auth.CreateUserWithEmailAndPasswordAsync(email, password);
            // 명시적으로 성공 이벤트를 여기서 보낼 수도 있지만, StateChanged에 의존하는 것이 더 일관적일 수 있음
            // OnAuthOperationCompleted?.Invoke(true, "회원가입 성공! 로그인됩니다.", signUpResult.User);
            // --> 성공 시에는 StateChanged가 OnUserSignedIn을 호출하므로, UI매니저는 그쪽에서 최종 처리를 함.
            // --> 여기서는 작업 시도 자체에 대한 피드백이 필요하다면 고려. (현재는 예외 처리에서만 실패 이벤트 호출)
            // --> 혹은, 성공/실패 관계없이 작업 완료만 알리는 이벤트를 추가할 수도 있음. (예: OnAttemptFinished)
        }
        catch (Exception signUpException)
        {
            if (signUpException.GetBaseException() is FirebaseException signUpFbEx)
            {
                AuthError errorCode = (AuthError)signUpFbEx.ErrorCode;
                if (errorCode == AuthError.EmailAlreadyInUse)
                {
                    Debug.Log($"Email {email} already in use. Attempting to sign in...");
                    try
                    {
                        AuthResult signInResult = await _auth.SignInWithEmailAndPasswordAsync(email, password);
                        // 성공 시 StateChanged가 OnUserSignedIn을 호출
                        // OnAuthOperationCompleted?.Invoke(true, "로그인 성공!", signInResult.User);
                    }
                    catch (Exception signInException)
                    {
                        string finalErrorMessage = "중복된 이메일로 가입을 시도했거나, 비밀번호가 틀렸습니다.";
                        if (signInException.GetBaseException() is FirebaseException signInFbEx_inner) {
                             Debug.LogError($"Sign-in failed for {email} (handler): {GetFirebaseAuthErrorMessage(signInFbEx_inner)} (Original: {signInFbEx_inner.Message})");
                        } else {
                             Debug.LogError($"Sign-in failed for {email} (handler) with non-Firebase exception: {signInException.Message}");
                        }
                        OnAuthOperationCompleted?.Invoke(false, finalErrorMessage, null);
                    }
                }
                else
                {
                    string errorMessage = GetFirebaseAuthErrorMessage(signUpFbEx);
                    Debug.LogError($"Sign-up failed for {email} (handler): {errorMessage} (Original: {signUpFbEx.Message})");
                    OnAuthOperationCompleted?.Invoke(false, $"회원가입 실패: {errorMessage}", null);
                }
            }
            else
            {
                string errorMessage = $"예상치 못한 오류 발생: {signUpException.Message}";
                Debug.LogError($"An unexpected error occurred for {email} (handler): {signUpException.ToString()}");
                OnAuthOperationCompleted?.Invoke(false, errorMessage, null);
            }
        }
    }

    private string GetFirebaseAuthErrorMessage(FirebaseException firebaseEx)
    {
        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
        string message;
        switch (errorCode)
        {
            case AuthError.Cancelled: message = "인증이 취소되었습니다."; break;
            case AuthError.InvalidEmail: message = "유효하지 않은 이메일 형식입니다."; break;
            case AuthError.WrongPassword: message = "이메일 또는 비밀번호가 일치하지 않습니다."; break;
            case AuthError.UserNotFound: message = "등록되지 않은 이메일입니다."; break;
            case AuthError.EmailAlreadyInUse: message = "이미 사용 중인 이메일입니다."; break;
            case AuthError.WeakPassword: message = "비밀번호는 6자 이상이어야 합니다."; break;
            case AuthError.NetworkRequestFailed: message = "네트워크 연결을 확인해주세요."; break;
            case AuthError.TooManyRequests: message = "요청 한도를 초과했습니다. 잠시 후 다시 시도해주세요."; break;
            case AuthError.UserDisabled: message = "사용 중지된 계정입니다."; break;
            default:
                Debug.LogWarning($"Unhandled Firebase AuthError in GetFirebaseAuthErrorMessage: {errorCode} (Original: {firebaseEx.Message})");
                message = $"인증 중 오류가 발생했습니다. (코드: {errorCode})"; break;
        }
        return message;
    }

    void OnDestroy()
    {
        if (FirebaseAuthenticator.Instance != null)
        {
            FirebaseAuthenticator.Instance.OnInitializationComplete -= HandleFirebaseInitForAuthHandler;
        }
    }
}