using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private float moveDirection = 0f;
    private bool isMoving = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 키 입력 감지
        moveDirection = 0f;
        isMoving = false;
        
        // 왼쪽 이동 (A 키)
        if (Input.GetKey(KeyCode.A) && rb != null && !rb.isKinematic)
        {
            moveDirection = -1f;
            isMoving = true;
        }
        
        // 오른쪽 이동 (D 키)
        if (Input.GetKey(KeyCode.D) && rb != null && !rb.isKinematic)
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