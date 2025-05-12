using UnityEngine;

public class TeleportObject : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform linkedTeleportPoint; // 연결된 텔레포트 지점
    public string interactionButton = "Interact"; // 상호작용 버튼 이름 (InputManager에서 설정)

    private bool isPlayerInRange = false; // 플레이어가 텔레포트 범위에 있는지 확인

    void Update()
    {
        // 플레이어가 범위 내에 있고 상호작용 버튼을 눌렀을 때
        if (isPlayerInRange && Input.GetButtonDown(interactionButton))
        {
            TeleportPlayer();
        }
    }

    public void TeleportPlayer() // 접근 제한자를 public으로 설정
    {
        if (linkedTeleportPoint != null)
        {
            // 플레이어를 연결된 텔레포트 지점으로 이동
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            player.position = linkedTeleportPoint.position;

            // 카메라도 플레이어를 따라서 이동
            Camera.main.transform.position = new Vector3(player.position.x, Camera.main.transform.position.y, player.position.z);

            Debug.Log("Player teleported to: " + linkedTeleportPoint.position);
        }
        else
        {
            Debug.LogWarning("No linked teleport point assigned.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player entered teleport range.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player left teleport range.");
        }
    }
}