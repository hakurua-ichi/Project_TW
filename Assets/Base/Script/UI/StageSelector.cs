using UnityEngine;
using UnityEngine.UI;

public class StageSelector : MonoBehaviour
{
    public Image backgroundImage;
    public Sprite[] stageBackgrounds;

    public GameObject subStagePopup;

    public void OnStageButtonClick(int stageIndex)
    {
        if (stageIndex < stageBackgrounds.Length)
        {
            backgroundImage.sprite = stageBackgrounds[stageIndex];
            subStagePopup.SetActive(true);
            subStagePopup.GetComponent<SubStageUI>().BuildSubStages(stageIndex + 1);
        }
    }
}