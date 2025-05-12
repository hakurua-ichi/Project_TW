using UnityEngine;

public class InteractionsButtonAction : MonoBehaviour
{
    [SerializeField] private GameObject ActionObject;
    private IGimmickObserver gimmick;

    void Start()
    {
        gimmick = ActionObject.GetComponent<IGimmickObserver>();
        if (gimmick == null)
        {
            Debug.LogWarning("ActionObject에 IGimmickObserver 구현체가 없습니다.");
        }
    }

    public void ButtonClicked()
    {
        gimmick?.ButtonClick();
    }
}