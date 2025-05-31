using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SubStageUI : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform buttonContainer;

    public void BuildSubStages(int stageNum)
    {
        // 기존 버튼 제거
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        int subCount = 3;
        for (int i = 1; i <= subCount; i++)
        {
            GameObject btn = Instantiate(buttonPrefab, buttonContainer);
            btn.GetComponentInChildren<TMP_Text>().text = $"{stageNum}-{i}";
            int s = stageNum, ss = i;
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                SceneManager.LoadScene($"stage{s}-{ss}"); //스테이지 이름 맞추어서 변경해주면 따라감
            });
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}