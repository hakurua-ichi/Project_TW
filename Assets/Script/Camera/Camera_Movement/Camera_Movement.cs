using UnityEngine;

public class Camera_Movement : MonoBehaviour
{
    private Transform playerTransform;
    private Transform cameraTransform;
    [SerializeField] private float rotationSpeed = 10f; // 회전 속도
    private bool isRotating = false; // 회전 중인지 확인
    private Quaternion targetRotation; // 목표 회전값


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        cameraTransform = transform; // 메인 카메라에 부착할 스크립트이기에 해당 코드도 유효하다.
        Setup(); // Setup 메서드 호출
        targetRotation = cameraTransform.rotation; // 초기 목표 회전값 설정

    }

    private void Setup()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform not found. Make sure the player has the 'Player' tag.");
        }
        if (cameraTransform == null)
        {
            Debug.LogError("Camera Transform not found. Make sure the camera is set up correctly.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 회전 중이 아니면 입력을 감지
        if (!isRotating)
        {
            if (Input.GetKey(KeyCode.R)) // R 키를 눌렀을 때
            {
                RotateCamera(-90f);
            }
            else if (Input.GetKey(KeyCode.T)) // T 키를 눌렀을 때
            {
                RotateCamera(90f);
            }
        }

        // 목표 회전값으로 부드럽게 회전
        if (isRotating)
        {
            cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // 목표 회전값에 도달했는지 확인
            if (Quaternion.Angle(cameraTransform.rotation, targetRotation) < 0.1f)
            {
                cameraTransform.rotation = targetRotation; // 정확히 목표값으로 설정
                isRotating = false; // 회전 종료
            }
        }
    }

    private void RotateCamera(float angle)
    {
        // 플레이어를 중심으로 카메라를 회전
        Vector3 direction = cameraTransform.position - playerTransform.position; // 플레이어와 카메라 간의 방향 벡터
        direction = Quaternion.Euler(0, angle, 0) * direction; // 방향 벡터를 회전
        cameraTransform.position = playerTransform.position + direction; // 새로운 위치 설정

        // 목표 회전값 설정
        targetRotation = Quaternion.LookRotation(playerTransform.position - cameraTransform.position);
        isRotating = true; // 회전 시작
    }
}
