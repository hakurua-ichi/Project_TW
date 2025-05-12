using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseAuthenticator : MonoBehaviour
{
    private FirebaseApp _firebaseApp;
    private FirebaseAuth _auth;
    private FirebaseUser _user;

    public FirebaseUser CurrentUser => _user;
    public bool IsInitialized { get; private set; } = false;

    public event Action<FirebaseUser> OnAuthStateChanged;
    public event Action<bool, string> OnInitializationCompleted;

    public void Initialize()
    {
        Debug.Log("FirebaseAuthenticator: Initializing Firebase...");
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                _firebaseApp = FirebaseApp.DefaultInstance;
                _auth = FirebaseAuth.DefaultInstance;
                _auth.StateChanged += HandleAuthStateChanged;
                HandleAuthStateChanged(this, null);
                IsInitialized = true;
                Debug.Log("FirebaseAuthenticator: Firebase Initialized successfully.");
                OnInitializationCompleted?.Invoke(true, "Firebase 초기화 성공");
            }
            else
            {
                IsInitialized = false;
                Debug.LogError($"FirebaseAuthenticator: Could not resolve all Firebase dependencies: {dependencyStatus}");
                OnInitializationCompleted?.Invoke(false, $"Firebase 초기화 실패: {dependencyStatus}");
            }
        });
    }

    private void HandleAuthStateChanged(object sender, EventArgs e)
    {
        if (_auth == null) return;

        if (_auth.CurrentUser != _user)
        {
            _user = _auth.CurrentUser;
            if (_user != null)
            {
                Debug.Log($"FirebaseAuthenticator: User signed in: {_user.Email} (UID: {_user.UserId})");
            }
            else
            {
                Debug.Log("FirebaseAuthenticator: User signed out.");
            }
            OnAuthStateChanged?.Invoke(_user);
        }
    }

    public async Task<(FirebaseUser User, AuthError ErrorCode, string ErrorMessage)> SignInUserAsync(string email, string password)
    {
        if (!IsInitialized || _auth == null)
        {
            Debug.LogError("FirebaseAuthenticator: Not initialized or auth instance is null.");
            return (null, AuthError.None, "Firebase not initialized.");
        }

        try
        {
            AuthResult authResult = await _auth.SignInWithEmailAndPasswordAsync(email, password).ConfigureAwait(true);
            FirebaseUser signedInUser = authResult.User;
            return (signedInUser, AuthError.None, null);
        }
        catch (Exception ex)
        {
            return HandleAuthException(ex);
        }
    }

    public async Task<(FirebaseUser User, AuthError ErrorCode, string ErrorMessage)> CreateUserAsync(string email, string password)
    {
        if (!IsInitialized || _auth == null)
        {
             Debug.LogError("FirebaseAuthenticator: Not initialized or auth instance is null.");
            return (null, AuthError.None, "Firebase not initialized.");
        }

        Debug.LogWarning("Firebase Auth는 사용자의 비밀번호를 안전하게 해시하여 저장합니다. 클라이언트에서 직접 암호화하지 마세요.");
        try
        {
            AuthResult authResult = await _auth.CreateUserWithEmailAndPasswordAsync(email, password).ConfigureAwait(true);
            FirebaseUser newUser = authResult.User;
            return (newUser, AuthError.None, null);
        }
        catch (Exception ex)
        {
            return HandleAuthException(ex);
        }
    }

    public void SignOut()
    {
        if (_auth != null && _auth.CurrentUser != null)
        {
            _auth.SignOut();
        }
        else
        {
            Debug.LogWarning("FirebaseAuthenticator: Auth instance is null or no user is currently signed in to sign out.");
        }
    }

    private (FirebaseUser, AuthError, string) HandleAuthException(Exception ex)
    {
        Debug.LogError($"FirebaseAuthenticator Error: {ex}");
        if (ex.GetBaseException() is FirebaseException firebaseEx)
        {
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            return (null, errorCode, firebaseEx.Message);
        }
        return (null, AuthError.None, ex.Message);
    }

    void OnDestroy()
    {
        if (_auth != null)
        {
            _auth.StateChanged -= HandleAuthStateChanged;
            _auth = null;
        }
        _firebaseApp = null;
    }
}