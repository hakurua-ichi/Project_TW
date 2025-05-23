using UnityEngine;

public class DoorAnimator
{
    private Animator animator;
    // 파라미터 해시 캐싱
    private static readonly int HashRightOpen = Animator.StringToHash("RightOpen");
    private static readonly int HashRightClose = Animator.StringToHash("RightClose");
    private static readonly int HashLeftOpen = Animator.StringToHash("LeftOpen");
    private static readonly int HashLeftClose = Animator.StringToHash("LeftClose");

    public DoorAnimator(GameObject doorObject)
    {
        animator = doorObject.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator가 문 오브젝트에 없습니다!");
        }
    }

    //아래 주석 처리된 내용은 트리거존을 통한 문 작동시 문이 열리는 방향을 결정하기 위한 코드
    public void Open(DoorState state)
    {
        if (animator == null) return;

        #region 트리거 존에 의한 작동
        /*
        Vector3 toPlayer = (state.Player.position - state.DoorTransform.position).normalized;
        float dot = Vector3.Dot(state.DoorTransform.right, toPlayer);

        if (dot > 0)
        {
            animator.SetTrigger("RightOpen");
        }
        else
        {
            animator.SetTrigger("LeftOpen");
        }
        */
        #endregion

        animator.SetTrigger(HashRightOpen);
    }

    public void Close(DoorState state)
    {
        if (animator == null) return;

        #region 트리거 존에 의한 작동
        /*
        Vector3 toPlayer = (state.Player.position - state.DoorTransform.position).normalized;
        float dot = Vector3.Dot(state.DoorTransform.right, toPlayer);

        if (dot > 0)
        {
            animator.SetTrigger("RightClose");
        }
        else
        {
            animator.SetTrigger("LeftClose");
        }
        */
        #endregion

        animator.SetTrigger(HashRightClose);
    }
}