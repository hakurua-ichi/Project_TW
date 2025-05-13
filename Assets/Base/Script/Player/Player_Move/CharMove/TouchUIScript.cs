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

    // 좌우 이동 상태
    private float currentMoveDirection = 0f;
    private bool isLeftPressed = false;
    private bool isRightPressed = false;    void Start()
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
            // 반시계 방향 (-90도) 회전 요청
            // RotationInput의 RequestRotation 메서드 호출
            cameraRotationInput.RequestRotation(-90f);
        }
    }
    
    private void OnRotateRightButtonPressed()
    {
        if (cameraRotationInput != null)
        {
            // 시계 방향 (90도) 회전 요청
            // RotationInput의 RequestRotation 메서드 호출
            cameraRotationInput.RequestRotation(90f);
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