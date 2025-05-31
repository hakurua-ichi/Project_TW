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

        int subCount = GetSubCount(stageNum);
        for (int i = 1; i <= subCount; i++)
        {
            GameObject btn = Instantiate(buttonPrefab, buttonContainer);
            btn.GetComponentInChildren<TMP_Text>().text = $"{stageNum}-{i}";
            int s = stageNum, ss = i;
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene($"Stage_{s}_{ss}"); //스테이지 이름 맞추어서 변경해주면 따라감
            });
        }
    }

    int GetSubCount(int stageNum)
    {
        return stageNum switch
        {
            1 => 3,
            2 => 2,
            _ => 1,
        };
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}