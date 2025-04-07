using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private Keyboard keyboard;
    private float moveDirection = 0f;
    private bool isMoving = false;
    private Rigidbody rb;

    void Start()
    {
        keyboard = Keyboard.current;
        if (keyboard == null)
        {
            Debug.LogError("키보드를 찾을 수 없습니다!");
        }
        
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (keyboard == null) return;
        
        // 키 입력 감지
        moveDirection = 0f;
        isMoving = false;
        
        // 왼쪽 이동 (A 키)
        if (keyboard.aKey.isPressed && rb != null && !rb.isKinematic)
        {
            moveDirection = -1f;
            isMoving = true;
        }
        
        // 오른쪽 이동 (D 키)
        if (keyboard.dKey.isPressed && rb != null && !rb.isKinematic)
        {
            moveDirection = 1f;
            isMoving = true;
        }
    }

    public float GetMoveDirection()
    {
        return moveDirection;
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}