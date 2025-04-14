using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // 따라갈 플레이어의 Transform을 지정하세요.
    public Transform player;
    // 카메라가 따라가는 속도 (부드러움 조절)
    public float followSpeed = 4f;
    // 플레이어와 카메라 간의 x, y 임계치. 이 범위를 벗어나면 카메라가 따라감.
    public Vector2 offsetThreshold = new Vector2(2f, 1f);

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void LateUpdate()
    {
        if (player == null)
            return;

        // 현재 카메라 위치
        Vector3 cameraPos = transform.position;
        // 플레이어 위치
        Vector3 playerPos = player.position;
        Vector3 newCameraPos = cameraPos;

        // 수평(가로) 방향 임계치 확인
        if (Mathf.Abs(playerPos.x - cameraPos.x) > offsetThreshold.x)
        {
            newCameraPos.x = Mathf.Lerp(cameraPos.x, playerPos.x, followSpeed * Time.deltaTime);
        }
        // 수직(세로) 방향 임계치 확인
        if (Mathf.Abs(playerPos.y + 2f - cameraPos.y) > offsetThreshold.y)
        {
            newCameraPos.y = Mathf.Lerp(cameraPos.y, playerPos.y, followSpeed * Time.deltaTime);
        }
        
        // z축은 카메라 고정 (원하는 경우 수정 가능)
        newCameraPos.z = cameraPos.z;

        transform.position = newCameraPos;
    }
}