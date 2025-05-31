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

    public event Action<bool, string> OnInitializationComplete;
    public event Action<FirebaseUser> OnUserSignedIn;
    public event Action OnUserSignedOut;

    private FirebaseUser _currentUserInternalCache;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFirebase();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            // 이 콜백을 메인 스레드에서 실행하도록 Dispatcher에 작업을 넘김
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                var dependencyStatus = task.Result; // task.Result는 여기서 접근해야 함
                if (dependencyStatus == DependencyStatus.Available)
                {
                    _firebaseApp = FirebaseApp.DefaultInstance;
                    _auth = FirebaseAuth.DefaultInstance;
                    _auth.StateChanged += AuthStateChanged;

                    IsInitialized = true;
                    OnInitializationComplete?.Invoke(true, "Firebase SDK 초기화 성공.");
                    AuthStateChanged(this, null);
                }
                else
                {
                    IsInitialized = false;
                    string errorMessage = $"FirebaseAuthenticator: Could not resolve all Firebase dependencies: {dependencyStatus} (from dispatched ContinueWith)";
                    Debug.LogError(errorMessage);
                    OnInitializationComplete?.Invoke(false, errorMessage);
                }
            });
        });
    }

    private void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        if (!IsInitialized || _auth == null)
        {
            // Debug.LogWarning("AuthStateChanged called but not initialized or auth is null."); // 너무 빈번할 수 있음
            return;
        }

        FirebaseUser sdkCurrentUser = _auth.CurrentUser;
        // Debug.Log($"AuthStateChanged - SDK User: {(sdkCurrentUser == null ? "NULL" : sdkCurrentUser.UserId)}, Cached User: {(_currentUserInternalCache == null ? "NULL" : _currentUserInternalCache.UserId)}");

        if (sdkCurrentUser != _currentUserInternalCache)
        {
            _currentUserInternalCache = sdkCurrentUser;
            if (_currentUserInternalCache != null)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => OnUserSignedIn?.Invoke(_currentUserInternalCache));
            }
            else
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => OnUserSignedOut?.Invoke());
            }
        }
    }

    public void SignOut()
    {
        if (_auth == null || _auth.CurrentUser == null)
        {
            Debug.LogWarning("FirebaseAuthenticator: No user to sign out or Auth not initialized.");
            return;
        }
        _auth.SignOut();
    }

    public FirebaseAuth GetFirebaseAuth()
    {
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