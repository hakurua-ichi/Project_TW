using UnityEngine;

public class OpenDoorAction : IGimmickAction
{
    private Animator animator;
    private bool isOpen = false;
    private Transform doorTransform;
    private Transform playerTransform; // 트리거에서 넘겨받을 예정

    public OpenDoorAction(GameObject doorObject, Transform player)
    {
        doorTransform = doorObject.transform;
        animator =  doorObject.GetComponent<Animator>();
        playerTransform = player;
        if (animator == null)
        {
            Debug.LogError("Animator가 문 오브젝트에 없습니다!");
        }
    }

    public void Action()
    {
        if(!isOpen)
        {
            Vector3 toPlayer = (playerTransform.position - doorTransform.position).normalized;
            float dot = Vector3.Dot(doorTransform.right, toPlayer);

            Debug.Log($"문 방향: {doorTransform.forward}, 플레이어 방향: {toPlayer}, dot: {dot}");

            if(dot > 0)
            {
                Debug.Log("오른쪽으로 문 열림 애니메이션 실행");
                animator.SetTrigger("RightOpen");
            }
            else
            {
                Debug.Log("왼쪽으로 문 열림 애니메이션 실행.");
                animator.SetTrigger("LeftOpen");
            }
            isOpen = true;
        }
        else
        {
            Debug.Log("문이 이미 열려있습니다.");
        }
    }

    public void Execute()
    {
        if(isOpen)
        {
            Vector3 toPlayer = (playerTransform.position - doorTransform.position).normalized;
            float dot = Vector3.Dot(doorTransform.right, toPlayer);

            if (dot > 0)
            {
                Debug.Log("오른쪽으로 문 닫힘 애니메이션 실행");
                animator.SetTrigger("RightClose");
            }
            else
            {
                Debug.Log("왼쪽으로 문 닫힘 애니메이션 실행.");
                animator.SetTrigger("LeftClose");
            }
            isOpen = false;
        }
        else
        {
            Debug.Log("문이 이미 닫혀있습니다.");
        }
    }

    public void setup()
    {
        isOpen = false;
    }
}
