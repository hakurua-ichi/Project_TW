using UnityEngine;

public class DoorAnimator : MonoBehaviour
{
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float speed = 1f;

    private Quaternion closeRotation;
    private Quaternion openRotation;

    private void Start()
    {

        closeRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, openAngle, 0f));
    }

    private void Update()
    {
        
    }

    //아래 주석 처리된 내용은 트리거존을 통한 문 작동시 문이 열리는 방향을 결정하기 위한 코드
    public void Open(DoorState state)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, openRotation, Time.deltaTime * speed);


        //if (animator == null) return;

        //#region 트리거 존에 의한 작동
        
        //Vector3 toPlayer = (state.Player.position - state.DoorTransform.position).normalized;
        //float dot = Vector3.Dot(state.DoorTransform.right, toPlayer);

        //if (dot > 0)
        //{
        //    animator.SetTrigger("RightOpen");
        //}
        //else
        //{
        //    animator.SetTrigger("LeftOpen");
        //}
        
        //#endregion

        //animator.SetTrigger(HashRightOpen);
    }

    public void Close(DoorState state)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, closeRotation, Time.deltaTime * speed);

        //if (animator == null) return;

        //#region 트리거 존에 의한 작동
        
        //Vector3 toPlayer = (state.Player.position - state.DoorTransform.position).normalized;
        //float dot = Vector3.Dot(state.DoorTransform.right, toPlayer);

        //if (dot > 0)
        //{
        //    animator.SetTrigger("RightClose");
        //}
        //else
        //{
        //    animator.SetTrigger("LeftClose");
        //}
        
        //#endregion

        //animator.SetTrigger(HashRightClose);
    }
}