using UnityEngine;

public class LaddersAction : IGimmickAction
{
    private LaddersState laddersState; // 사다리 상태 관리
    private LaddersAnimator laddersAnimator; // 사다리 애니메이션 관리

    public LaddersAction(LaddersState LaddersState)
    {
        laddersState = LaddersState; // 사다리 상태 초기화
    }

    public void Action()
    {
        int playerPositionY = Mathf.RoundToInt(laddersState.Player.transform.position.y); // 플레이어의 Y 좌표
        int topPointY = Mathf.RoundToInt(laddersState.topPoint.transform.position.y); // 사다리의 위쪽 포인트 Y 좌표
        int bottomPointY = Mathf.RoundToInt(laddersState.bottomPoint.transform.position.y); // 사다리의 아래쪽 포인트 Y 좌표

        // 플레이어가 사다리 위쪽에 있을 때
        if (playerPositionY == topPointY)
        {
            // 아래쪽 포인트로 이동
            laddersState.Player.transform.position = new Vector3(
                laddersState.Player.transform.position.x,
                laddersState.bottomPoint.transform.position.y,
                laddersState.Player.transform.position.z
            ); 
        }

        // 플레이어가 사다리 아래쪽에 있을 때
        else
        {
            // 위쪽 포인트로 이동
            laddersState.Player.transform.position = new Vector3(
                laddersState.Player.transform.position.x,
                laddersState.topPoint.transform.position.y,
                laddersState.Player.transform.position.z
            );
        }
    }

    public void Exit() { }

    public void Setup()
    {
        // 사다리 타기 상태 초기화
        Debug.Log("사다리 타기 상태를 초기화합니다.");
    }
}
