using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string runParameterName = "isRunning";
    
    private bool isRunning = false;

    void Start()
    {
        // 애니메이터 참조 가져오기 (지정되지 않은 경우)
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
                if (animator == null)
                {
                    Debug.LogWarning("애니메이터 컴포넌트를 찾을 수 없습니다. 애니메이션이 작동하지 않을 수 있습니다.");
                }
            }
        }
    }

    public void UpdateAnimationState(bool isMoving)
    {
        if (animator != null && this.isRunning != isMoving)
        {
            this.isRunning = isMoving;
            animator.SetBool(runParameterName, isMoving);
        }
    }
}