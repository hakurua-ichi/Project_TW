using UnityEngine;

public class DoorAnimator : MonoBehaviour
{
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float speed = 1f;
    private bool doorState;

    private Quaternion closeRotation;
    private Quaternion openRotation;

    private void Start()
    {

        closeRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, openAngle, 0f));
    }

    private void Update()
    {
        if(doorState)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, openRotation, Time.deltaTime * speed);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, closeRotation, Time.deltaTime * speed);
        }
    }

    //아래 주석 처리된 내용은 트리거존을 통한 문 작동시 문이 열리는 방향을 결정하기 위한 코드
    public void SetState(DoorState state)
    {
        this.doorState = state.IsOpen;
    }
}