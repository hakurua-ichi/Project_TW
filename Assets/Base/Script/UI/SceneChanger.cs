using System;
using UnityEngine;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private String SceneName;

    void Start()
    {
        // 시작 시 버튼에 OnClickLoadScene 메소드를 연결
        if(button != null)
        {
            button.onClick.AddListener(OnClickLoadScene);
        }
    }
    
    // 인스펙터에서 버튼 이벤트에 연결할 수 있도록 public으로 변경
    public void OnClickLoadScene()
    {
        // 씬 로드
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
    }
}
