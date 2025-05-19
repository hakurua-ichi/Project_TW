using UnityEngine;
using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase;

public class EmailPasswordAuthHandler : MonoBehaviour
{
    /// <summary>
    /// 살아-있는 EmailPasswordAuthHandler를 어디서든 얻을 수 있는 싱글톤 참조
    /// </summary>
    public static EmailPasswordAuthHandler Instance { get; private set; }

    private FirebaseAuth _auth;

    // 이벤트 정의
    /// <summary>
    /// 이메일/비밀번호 인증 시도(가입 또는 로그인) 완료 시 발생 (성공 여부, 메시지, 사용자 정보)
    /// </summary>
    public event Action<bool, string, FirebaseUser> OnAuthOperationCompleted;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // AuthService 오브젝트 자체에 DontDestroyOnLoad가 이미 들어가 있다면
            // 여기서는 굳이 또 호출할 필요 없습니다.
            // DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            // 중복 생성되면 바로 파괴해 레퍼런스 혼란 방지
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // FirebaseAuthenticator가 초기화될 시간을 기다리거나,
        // FirebaseAuthenticator의 OnInitializationComplete 이벤트를 구독하여 _auth를 설정할 수 있습니다.
        // 여기서는 간단하게 Instance를 통해 가져옵니다.
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
            Debug.LogError("EmailPasswordAuthHandler: FirebaseAuthenticator instance not found.");
        }
    }

    private void HandleFirebaseInitialization(bool success, string message)
    {
        if (success && FirebaseAuthenticator.Instance != null)
        {
            _auth = FirebaseAuthenticator.Instance.GetFirebaseAuth();
            FirebaseAuthenticator.Instance.OnInitializationComplete -= HandleFirebaseInitialization; // 이벤트 구독 해제
        }
        else
        {
            Debug.LogError($"EmailPasswordAuthHandler: Firebase initialization failed. Cannot get FirebaseAuth. Message: {message}");
        }
    }

    /// <summary>
    /// 이메일과 비밀번호로 새 사용자를 등록합니다.
    /// </summary>
    public async Task SignUpWithEmailAsync(string email, string password)
    {
        if (_auth == null)
        {
            OnAuthOperationCompleted?.Invoke(false, "Firebase 인증이 초기화되지 않았습니다.", null);
            return;
        }

        try
        {
            AuthResult authResult = await _auth.CreateUserWithEmailAndPasswordAsync(email, password);
            FirebaseUser newUser = authResult.User;
            Debug.Log($"EmailPasswordAuthHandler: User signed up successfully: {newUser.UserId} ({newUser.Email})");
            OnAuthOperationCompleted?.Invoke(true, "회원가입 성공!", newUser);
        }
        catch (Exception e)
        {
            string errorMessage = GetFirebaseAuthErrorMessage(e);
            Debug.LogError($"EmailPasswordAuthHandler: Email sign-up failed: {errorMessage}");
            OnAuthOperationCompleted?.Invoke(false, $"회원가입 실패: {errorMessage}", null);
        }
    }

    /// <summary>
    /// 이메일과 비밀번호로 기존 사용자를 로그인합니다.
    /// </summary>
    public async Task SignInWithEmailAsync(string email, string password)
    {
        if (_auth == null)
        {
            OnAuthOperationCompleted?.Invoke(false, "Firebase 인증이 초기화되지 않았습니다.", null);
            return;
        }

        try
        {
            AuthResult authResult = await _auth.SignInWithEmailAndPasswordAsync(email, password);
            FirebaseUser signedInUser = authResult.User;
            Debug.Log($"EmailPasswordAuthHandler: User signed in successfully: {signedInUser.UserId} ({signedInUser.Email})");
            OnAuthOperationCompleted?.Invoke(true, "로그인 성공!", signedInUser);
        }
        catch (Exception e)
        {
            string errorMessage = GetFirebaseAuthErrorMessage(e);
            Debug.LogError($"EmailPasswordAuthHandler: Email sign-in failed: {errorMessage}");
            OnAuthOperationCompleted?.Invoke(false, $"로그인 실패: {errorMessage}", null);
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
                case AuthError.WrongPassword:
                    return "잘못된 비밀번호입니다.";
                case AuthError.UserNotFound:
                    return "등록되지 않은 이메일입니다.";
                case AuthError.InvalidEmail:
                    return "유효하지 않은 이메일 형식입니다.";
                case AuthError.EmailAlreadyInUse:
                    return "이미 사용 중인 이메일입니다.";
                case AuthError.WeakPassword:
                    return "비밀번호는 6자 이상이어야 합니다.";
                // 기타 필요한 오류 메시지들...
                default:
                    return $"오류 코드: {errorCode}"; // 기본 Firebase 오류 메시지 대신 코드만 표시
            }
        }
        return e.Message; // 일반 예외 메시지
    }

    void OnDestroy()
    {
        if (FirebaseAuthenticator.Instance != null)
        {
            FirebaseAuthenticator.Instance.OnInitializationComplete -= HandleFirebaseInitialization;
        }
    }
}