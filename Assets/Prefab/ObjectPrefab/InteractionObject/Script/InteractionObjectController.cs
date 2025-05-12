using UnityEngine;

public class InteractionObjectController : MonoBehaviour, IGimmickObserver
{
    [SerializeField] private GameObject buttonUI;
    [SerializeField] private bool showOnEnter = false; // true: Enter에 표시, false: Exit에 숨김

    private void Awake()
    {
        if (buttonUI != null)
        {
            buttonUI.SetActive(false);
        }
            
        else
            Debug.LogWarning("UIButtonAdapter: buttonUI가 설정되지 않았습니다.");
    }

    private void Start()
    {
        buttonUI.SetActive(false);
    }

    public void OnGimmickTriggered()
    {
        if (buttonUI == null) return;

        buttonUI.SetActive(showOnEnter);
    }

    public static InteractionObjectController Attach(GameObject targetObject, GameObject buttonUI, bool showOnEnter)
    {
        var adapter = targetObject.AddComponent<InteractionObjectController>();
        adapter.buttonUI = buttonUI;
        adapter.showOnEnter = showOnEnter;
        return adapter;
    }
}