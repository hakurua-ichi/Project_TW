using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 터치 UI 버튼으로 캐릭터 좌우 이동 및 카메라 회전을 제어하는 스크립트
/// </summary>
public class TouchUIScript : MonoBehaviour
{
    [Header("이동 버튼")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    
    [Header("카메라 회전 버튼")]
    [SerializeField] private Button rotateLeftButton;  // 카메라 반시계방향 회전
    [SerializeField] private Button rotateRightButton; // 카메라 시계방향 회전
    
    [Header("참조")]
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private RotationInput cameraRotationInput;
    [SerializeField] private Transform playerTransform; // 플레이어 트랜스폼 참조
    [SerializeField] private string blockTag = "RotatingBlock"; // 회전 블록 태그
    [SerializeField] private float gridSize = 2.0f; // 그리드 크기

    // 좌우 이동 상태
    private float currentMoveDirection = 0f;
    private bool isLeftPressed = false;
    private bool isRightPressed = false;
    
    // 캐릭터 위치 최적화 관련
    private Rigidbody playerRigidbody;    void Start()
    {
        // 플레이어 입력 핸들러 찾기
        if (inputHandler == null)
        {
            inputHandler = FindAnyObjectByType<PlayerInputHandler>();
            if (inputHandler == null)
            {
                Debug.LogError("PlayerInputHandler를 찾을 수 없습니다. 스크립트가 비활성화되었습니다.");
                enabled = false;
                return;
            }
        }
        
        // 카메라 회전 입력 찾기
        if (cameraRotationInput == null)
        {
            cameraRotationInput = FindAnyObjectByType<RotationInput>();
            if (cameraRotationInput == null)
            {
                Debug.LogWarning("RotationInput을 찾을 수 없습니다. 카메라 회전 기능이 비활성화됩니다.");
            }
        }
        
        // 플레이어 트랜스폼 찾기
        if (playerTransform == null && inputHandler != null)
        {
            playerTransform = inputHandler.transform;
        }
        
        // 플레이어의 Rigidbody 참조
        if (playerTransform != null)
        {
            playerRigidbody = playerTransform.GetComponent<Rigidbody>();
        }

        // 버튼에 이벤트 리스너 추가
        SetupButtonEvents();
    }

    /// <summary>
    /// 버튼 이벤트 설정
    /// </summary>
    private void SetupButtonEvents()
    {
        // 왼쪽 이동 버튼 설정
        if (leftButton != null)
        {
            // 누르는 이벤트
            EventTrigger leftTrigger = leftButton.gameObject.GetComponent<EventTrigger>();
            if (leftTrigger == null)
                leftTrigger = leftButton.gameObject.AddComponent<EventTrigger>();

            AddEventTrigger(leftTrigger, EventTriggerType.PointerDown, (data) => { OnLeftButtonDown(); });
            AddEventTrigger(leftTrigger, EventTriggerType.PointerUp, (data) => { OnLeftButtonUp(); });
            AddEventTrigger(leftTrigger, EventTriggerType.PointerExit, (data) => { OnLeftButtonUp(); });
        }
        else
        {
            Debug.LogWarning("왼쪽 이동 버튼이 설정되지 않았습니다.");
        }        // 오른쪽 이동 버튼 설정
        if (rightButton != null)
        {
            EventTrigger rightTrigger = rightButton.gameObject.GetComponent<EventTrigger>();
            if (rightTrigger == null)
                rightTrigger = rightButton.gameObject.AddComponent<EventTrigger>();
                
            AddEventTrigger(rightTrigger, EventTriggerType.PointerDown, (data) => { OnRightButtonDown(); });
            AddEventTrigger(rightTrigger, EventTriggerType.PointerUp, (data) => { OnRightButtonUp(); });
            AddEventTrigger(rightTrigger, EventTriggerType.PointerExit, (data) => { OnRightButtonUp(); });
        }
        else
        {
            Debug.LogWarning("오른쪽 이동 버튼이 설정되지 않았습니다.");
        }
        
        // 카메라 회전 버튼 설정
        SetupCameraRotationButtons();
    }

    /// <summary>
    /// 카메라 회전 버튼 설정
    /// </summary>
    private void SetupCameraRotationButtons()
    {
        // 카메라 반시계 방향(왼쪽) 회전 버튼 설정
        if (rotateLeftButton != null)
        {
            EventTrigger rotateLTrigger = rotateLeftButton.gameObject.GetComponent<EventTrigger>();
            if (rotateLTrigger == null)
                rotateLTrigger = rotateLeftButton.gameObject.AddComponent<EventTrigger>();
                
            AddEventTrigger(rotateLTrigger, EventTriggerType.PointerDown, (data) => { OnRotateLeftButtonPressed(); });
        }
        else
        {
            Debug.LogWarning("카메라 왼쪽 회전 버튼이 설정되지 않았습니다.");
        }
        
        // 카메라 시계 방향(오른쪽) 회전 버튼 설정
        if (rotateRightButton != null)
        {
            EventTrigger rotateRTrigger = rotateRightButton.gameObject.GetComponent<EventTrigger>();
            if (rotateRTrigger == null)
                rotateRTrigger = rotateRightButton.gameObject.AddComponent<EventTrigger>();
                
            AddEventTrigger(rotateRTrigger, EventTriggerType.PointerDown, (data) => { OnRotateRightButtonPressed(); });
        }
        else
        {
            Debug.LogWarning("카메라 오른쪽 회전 버튼이 설정되지 않았습니다.");
        }
    }

    /// <summary>
    /// 이벤트 트리거 헬퍼 메서드
    /// </summary>
    private void AddEventTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    // 버튼 이벤트 핸들러
    private void OnLeftButtonDown()
    {
        isLeftPressed = true;
        UpdateMoveDirection();
    }

    private void OnLeftButtonUp()
    {
        isLeftPressed = false;
        UpdateMoveDirection();
    }

    private void OnRightButtonDown()
    {
        isRightPressed = true;
        UpdateMoveDirection();
    }

    private void OnRightButtonUp()
    {
        isRightPressed = false;
        UpdateMoveDirection();
    }    // 카메라 회전 버튼 이벤트 핸들러
    private void OnRotateLeftButtonPressed()
    {
        if (cameraRotationInput != null)
        {
            // 캐릭터를 블록의 중앙으로 이동
            MoveCharacterToGridCenter();
            
            // 반시계 방향 (-90도) 회전 요청
            cameraRotationInput.RequestRotation(-90f);
        }
    }
    
    private void OnRotateRightButtonPressed()
    {
        if (cameraRotationInput != null)
        {
            // 캐릭터를 블록의 중앙으로 이동
            MoveCharacterToGridCenter();
            
            // 시계 방향 (90도) 회전 요청
            cameraRotationInput.RequestRotation(90f);
        }
    }
    
    /// <summary>
    /// 캐릭터를 그리드의 중앙으로 이동시키는 메서드
    /// </summary>
    private void MoveCharacterToGridCenter()
    {
        // 플레이어 트랜스폼 유효성 검사
        if (playerTransform == null)
        {
            Debug.LogWarning("플레이어 트랜스폼이 설정되지 않았습니다.");
            return;
        }
        
        // 현재 캐릭터 위치
        Vector3 currentPosition = playerTransform.position;
        
        // 그리드 크기에 맞추어 중앙 좌표 계산
        float centerX = Mathf.Round(currentPosition.x / gridSize) * gridSize;
        float centerZ = Mathf.Round(currentPosition.z / gridSize) * gridSize;
        
        // Y 좌표는 유지
        Vector3 centerPosition = new Vector3(centerX, currentPosition.y, centerZ);
        
        // 디버그 로그
        Debug.Log($"캐릭터 위치 조정: {currentPosition} -> {centerPosition}");
        
        // 위치 이동 방식 결정 (리지드바디가 있는 경우 물리 기반 이동)
        if (playerRigidbody != null)
        {
            // 리지드바디 속도 초기화 (부드러운 이동을 위해)
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.position = centerPosition;
            
            // 회전 중 무한 상승 방지를 위해 Y축 속도 초기화
            Vector3 currentVelocity = playerRigidbody.linearVelocity;
            playerRigidbody.linearVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
        }
        else
        {
            // 트랜스폼 직접 이동
            playerTransform.position = centerPosition;
        }
        
        // 특수 블록 위에 있는지 확인 및 처리
        CheckForSpecialBlock();
    }
    
    /// <summary>
    /// 특수 블록 (회전 블록 등) 위에 있는지 확인하는 메서드
    /// </summary>
    private void CheckForSpecialBlock()
    {
        // 레이캐스트로 아래 블록 확인
        if (Physics.Raycast(playerTransform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 2.0f))
        {
            // 회전 블록인지 확인
            if (hit.collider.CompareTag(blockTag))
            {
                Debug.Log("회전 블록 위에서 회전합니다.");
                
                // 회전 블록의 중앙으로 정확히 이동 (추가 정밀 조정)
                Transform blockTransform = hit.collider.transform;
                Vector3 blockCenter = blockTransform.position;
                Vector3 adjustedPosition = new Vector3(blockCenter.x, playerTransform.position.y, blockCenter.z);
                
                if (playerRigidbody != null)
                {
                    playerRigidbody.position = adjustedPosition;
                }
                else
                {
                    playerTransform.position = adjustedPosition;
                }
            }
        }
    }

    /// <summary>
    /// 이동 방향 업데이트
    /// </summary>
    private void UpdateMoveDirection()
    {
        // 양쪽 버튼이 모두 눌린 경우 움직이지 않음
        if (isLeftPressed && isRightPressed)
        {
            currentMoveDirection = 0f;
        }
        // 왼쪽 버튼만 눌린 경우
        else if (isLeftPressed)
        {
            currentMoveDirection = -1f;
        }
        // 오른쪽 버튼만 눌린 경우
        else if (isRightPressed)
        {
            currentMoveDirection = 1f;
        }
        // 아무 버튼도 눌리지 않은 경우
        else
        {
            currentMoveDirection = 0f;
        }
        
        // 현재 이동 방향을 PlayerInputHandler에 알림
        inputHandler.SetTouchMoveDirection(currentMoveDirection);
    }
}