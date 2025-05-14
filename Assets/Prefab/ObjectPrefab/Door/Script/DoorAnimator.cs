using UnityEngine;

public class DoorAnimator
{
    private Animator animator;

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
        //Vector3 toPlayer = (state.Player.position - state.DoorTransform.position).normalized;
        //float dot = Vector3.Dot(state.DoorTransform.right, toPlayer);

        //if (dot > 0)
        //{
            animator.SetTrigger("RightOpen");
        //}
        //else
        //{
            //animator.SetTrigger("LeftOpen");
        //}
    }

    public void Close(DoorState state)
    {
        //Vector3 toPlayer = (state.Player.position - state.DoorTransform.position).normalized;
        //float dot = Vector3.Dot(state.DoorTransform.right, toPlayer);

        //if (dot > 0)
        //{
            animator.SetTrigger("RightClose");
        //}
        //else
        //{
            //animator.SetTrigger("LeftClose");
        //}
    }
}