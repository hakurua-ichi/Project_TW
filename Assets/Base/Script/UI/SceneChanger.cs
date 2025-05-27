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
        // 씬 이름 없을경우 에러 발생.
        if (string.IsNullOrEmpty(SceneName))
        {
            Debug.LogError("씬 이름이 비어있습니다. 씬 이름을 설정해주세요.");
            return;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
    }
}
