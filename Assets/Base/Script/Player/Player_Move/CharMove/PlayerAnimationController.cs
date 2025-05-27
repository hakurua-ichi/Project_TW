using UnityEngine;

/// <summary>
/// PlayerAnimationController
/// 
/// 플레이어 캐릭터의 애니메이션 상태를 관리하는 컴포넌트
/// 이동, 점프, 회전 등의 상태에 따라 적절한 애니메이션을 재생
/// 
/// 주요 기능:
/// - 이동/정지 애니메이션 상태 전환
/// - 애니메이터 파라미터 제어
/// - 플레이어 상태와 애니메이션 동기화
/// </summary>
public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string runParameterName = "isRunning";
    [SerializeField] private string jumpParameterName = "isJumping";
    
    private bool isRunning = false;
    private bool isJumping = false;

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

    /// <summary>
    /// 캐릭터의 이동 상태에 따라 애니메이션 상태 업데이트
    /// </summary>
    /// <param name="isMoving">캐릭터가 이동 중인지 여부</param>
    public void UpdateAnimationState(bool isMoving)
    {
        if (animator != null && this.isRunning != isMoving)
        {
            this.isRunning = isMoving;
            animator.SetBool(runParameterName, isMoving);
        }
    }
    
    /// <summary>
    /// 점프 애니메이션 상태 설정
    /// </summary>
    /// <param name="jumping">점프 중인지 여부</param>
    public void SetJumping(bool jumping)
    {
        if (animator != null && this.isJumping != jumping)
        {
            this.isJumping = jumping;
            animator.SetBool(jumpParameterName, jumping);
        }
    }
    
    /// <summary>
    /// 애니메이션 트리거 실행
    /// </summary>
    /// <param name="triggerName">트리거 파라미터 이름</param>
    public void TriggerAnimation(string triggerName)
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
    }
}
