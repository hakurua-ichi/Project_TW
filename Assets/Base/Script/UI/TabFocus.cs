using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하기 위해 추가

public class TabFocus : MonoBehaviour
{
    private TMP_InputField userIDInput;
    private TMP_InputField passwordInput;
    private Button registerButton;
    private Button loginButton;

    public void Initialize(TMP_InputField userIDInput, TMP_InputField passwordInput, Button registerButton, Button loginButton)
    {
        this.userIDInput = userIDInput;
        this.passwordInput = passwordInput;
        this.registerButton = registerButton;
        this.loginButton = loginButton;
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == userIDInput.gameObject && Input.GetKeyDown(KeyCode.Tab))
        {
            // Tab 키를 눌렀을 때 포커스를 비밀번호 입력란으로 이동
            passwordInput.Select();
        }
        else if (EventSystem.current.currentSelectedGameObject == passwordInput.gameObject && Input.GetKeyDown(KeyCode.Tab))
        {
            // Tab 키를 눌렀을 때 포커스를 아이디 입력란으로 이동
            registerButton.Select();
        }
        else if (EventSystem.current.currentSelectedGameObject == registerButton.gameObject && Input.GetKeyDown(KeyCode.Tab))
        {
            // Tab 키를 눌렀을 때 포커스를 로그인 버튼으로 이동
            loginButton.Select();
        }
        else if (EventSystem.current.currentSelectedGameObject == loginButton.gameObject && Input.GetKeyDown(KeyCode.Tab))
        {
            // Tab 키를 눌렀을 때 포커스를 아이디 입력란으로 이동
            userIDInput.Select();
        }
    }
}