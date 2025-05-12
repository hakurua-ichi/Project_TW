using UnityEngine;
using UnityEngine.UI;

public class InteractionUIButton : MonoBehaviour
{
    public Button interactButton; // UI 버튼

    private TeleportObject currentTeleportObject;

    void Start()
    {
        // 버튼 클릭 이벤트에 함수 연결
        interactButton.onClick.AddListener(OnInteractButtonClicked);
        interactButton.gameObject.SetActive(false); // 초기에는 비활성화
    }

    void Update()
    {
        // 텔레포트 오브젝트 주변에 있을 때 버튼 활성화
        if (currentTeleportObject != null)
        {
            interactButton.gameObject.SetActive(true);
        }
        else
        {
            interactButton.gameObject.SetActive(false);
        }
    }

    private void OnInteractButtonClicked()
    {
        if (currentTeleportObject != null)
        {
            currentTeleportObject.TeleportPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Teleport"))
        {
            currentTeleportObject = other.GetComponent<TeleportObject>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Teleport"))
        {
            currentTeleportObject = null;
        }
    }
}