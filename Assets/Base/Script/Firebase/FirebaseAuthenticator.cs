using UnityEngine;
using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;

public class FirebaseAuthenticator : MonoBehaviour
{
    public static FirebaseAuthenticator Instance { get; private set; }

    private FirebaseApp _firebaseApp;
    private FirebaseAuth _auth;

    public FirebaseUser CurrentUser => _auth?.CurrentUser;
    public bool IsInitialized { get; private set; } = false;

    // 이벤트 정의
    /// <summary>
    /// Firebase 초기화 완료 시 발생 (성공 여부, 메시지)
    /// </summary>
    public event Action<bool, string> OnInitializationComplete;

    /// <summary>
    /// 사용자 로그인 시 발생 (로그인된 사용자 정보)
    /// </summary>
    public event Action<FirebaseUser> OnUserSignedIn;

    /// <summary>
    /// 사용자 로그아웃 시 발생
    /// </summary>
    public event Action OnUserSignedOut;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 앱 전체에서 사용 가능하도록
            InitializeFirebase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                _firebaseApp = FirebaseApp.DefaultInstance;
                _auth = FirebaseAuth.DefaultInstance;
                _auth.StateChanged += AuthStateChanged; // 인증 상태 변경 리스너 등록

                IsInitialized = true;
                Debug.Log("FirebaseAuthenticator: Firebase SDK initialization successful.");
                OnInitializationComplete?.Invoke(true, "Firebase SDK 초기화 성공.");
                AuthStateChanged(this, null); // 초기 상태 강제 호출하여 현재 사용자 상태 반영
            }
            else
            {
                IsInitialized = false;
                Debug.LogError($"FirebaseAuthenticator: Could not resolve all Firebase dependencies: {dependencyStatus}");
                OnInitializationComplete?.Invoke(false, $"Firebase 종속성 해결 실패: {dependencyStatus}");
            }
        }, TaskScheduler.FromCurrentSynchronizationContext()); // Unity 메인 스레드에서 ContinueWith 실행 보장
    }

    private void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        if (!IsInitialized) return; // 초기화 전에는 호출 방지

        if (_auth.CurrentUser != CurrentUserInternalCache) // 실제 상태와 내부 캐시 비교 (중복 호출 방지 및 명확한 상태 변경 감지)
        {
            if (_auth.CurrentUser != null)
            {
                CurrentUserInternalCache = _auth.CurrentUser;
                Debug.Log($"FirebaseAuthenticator: User signed in: {CurrentUserInternalCache.UserId}");
                OnUserSignedIn?.Invoke(CurrentUserInternalCache);
            }
            else
            {
                CurrentUserInternalCache = null;
                Debug.Log("FirebaseAuthenticator: User signed out.");
                OnUserSignedOut?.Invoke();
            }
        } else if (_auth.CurrentUser != null && CurrentUserInternalCache == null) {
            // 앱 시작 시 이미 로그인 된 경우 (StateChanged가 CurrentUser가 null이 아닐 때 한번만 호출될 수 있음)
             CurrentUserInternalCache = _auth.CurrentUser;
             Debug.Log($"FirebaseAuthenticator: User already signed in: {CurrentUserInternalCache.UserId}");
             OnUserSignedIn?.Invoke(CurrentUserInternalCache);
        }
    }

    // StateChanged 이벤트에서 중복 호출을 방지하거나 명확한 변경을 감지하기 위한 내부 캐시
    private FirebaseUser CurrentUserInternalCache;


    /// <summary>
    /// Firebase에서 로그아웃합니다.
    /// Google 로그인 사용자는 Google Sign-Out도 함께 처리해야 할 수 있습니다 (GoogleAuthHandler에서 담당).
    /// </summary>
    public void SignOut()
    {
        if (_auth == null || _auth.CurrentUser == null)
        {
            Debug.LogWarning("FirebaseAuthenticator: No user to sign out or Auth not initialized.");
            return;
        }
        _auth.SignOut(); // StateChanged 이벤트가 호출되어 OnUserSignedOut이 트리거됨
    }


    /// <summary>
    /// FirebaseAuth 인스턴스를 반환합니다. 다른 AuthHandler에서 사용됩니다.
    /// 초기화 확인 후 사용해야 합니다.
    /// </summary>
    public FirebaseAuth GetFirebaseAuth()
    {
        if (!IsInitialized)
        {
            Debug.LogError("FirebaseAuthenticator: FirebaseAuth instance requested before initialization.");
            return null;
        }
        return _auth;
    }

    void OnDestroy()
    {
        if (_auth != null)
        {
            _auth.StateChanged -= AuthStateChanged;
            _auth = null;
        }
        if (Instance == this)
        {
            Instance = null;
        }
    }
}