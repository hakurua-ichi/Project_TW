using UnityEngine;

public class LaddersAction : IGimmickAction
{
    private LaddersState laddersState;

    public LaddersAction(LaddersState state)
    {
        laddersState = state;
    }

    public void Setup()
    {
        Debug.Log("사다리 탑승 준비 완료");
    }

    public void Exit() { }

    public void Action()
    {
        if (laddersState.Player == null ||
            laddersState.topPoint == null ||
            laddersState.bottomPoint == null)
        {
            Debug.LogWarning("LaddersAction: Player 또는 포인트가 null입니다.");
            return;
        }

        int playerPositionY = Mathf.RoundToInt(laddersState.Player.transform.position.y); // 플레이어의 Y 좌표
        int topPointY = Mathf.RoundToInt(laddersState.topPoint.transform.position.y); // 사다리의 위쪽 포인트 Y 좌표
        int bottomPointY = Mathf.RoundToInt(laddersState.bottomPoint.transform.position.y); // 사다리의 아래쪽 포인트 Y 좌표


        float highY = Mathf.Max(topPointY, bottomPointY);
        float lowY = Mathf.Min(topPointY, bottomPointY);

        // 위쪽에 있으면 아래로, 아래쪽에 있으면 위로
        if (playerPositionY >= highY - 0.1f)
        {
            MovePlayerToY(laddersState.Player, lowY);
            Debug.Log($"아래로 이동: {playerPositionY:F2} → {lowY:F2}");
        }
        else if (playerPositionY <= lowY + 0.1f)
        {
            MovePlayerToY(laddersState.Player, highY);
            Debug.Log($"위로 이동: {playerPositionY:F2} → {highY:F2}");
        }
        else
        {
            Debug.LogWarning($"사다리 중간에서 Action() 호출됨 (playerY={playerPositionY:F2})");
        }
    }

    /// <summary>
    /// CharacterController 또는 Rigidbody를 고려해서
    /// 플레이어를 targetY로 안전하게 이동시키는 헬퍼 메서드입니다.
    /// </summary>
    private void MovePlayerToY(GameObject playerObj, float targetY)
    {
        // 1) PlayerObj 또는 자식에서 CharacterController 찾기
        CharacterController cc = playerObj.GetComponent<CharacterController>();
        if (cc == null)
            cc = playerObj.GetComponentInChildren<CharacterController>();

        // 2) 이동 스크립트(예: PlayerMovement)가 있다면 잠시 꺼두기
        var customMover = playerObj.GetComponent<CharacterController>();
        if (customMover != null) customMover.enabled = false;

        if (cc != null)
        {
            // CharacterController가 붙어 있으면 잠깐 비활성화 후 이동
            cc.enabled = false;
            playerObj.transform.position = new Vector3(
                playerObj.transform.position.x,
                targetY,
                playerObj.transform.position.z
            );
            Debug.Log($"(사다리) 이동 직후 Y: {playerObj.transform.position.y:F2}");
            cc.enabled = true;
        }
        else
        {
            // Rigidbody가 붙어 있고 isKinematic=false라면 잠깐 Kinematic으로 전환 후 이동
            Rigidbody rb = playerObj.GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic)
            {
                rb.isKinematic = true;
                playerObj.transform.position = new Vector3(
                    playerObj.transform.position.x,
                    targetY,
                    playerObj.transform.position.z
                );
                Debug.Log($"(사다리) 이동 직후 Y: {playerObj.transform.position.y:F2}");
                rb.isKinematic = false;
            }
            else
            {
                // 그 외에는 그냥 Transform 위치만 이동
                playerObj.transform.position = new Vector3(
                    playerObj.transform.position.x,
                    targetY,
                    playerObj.transform.position.z
                );
                Debug.Log($"(사다리) 이동 직후 Y: {playerObj.transform.position.y:F2}");
            }
        }

        // 다시 이동 스크립트 켜기
        if (customMover != null) customMover.enabled = true;
    }
}

//public class LaddersAction : IGimmickAction
//{
//    private LaddersState laddersState; // 사다리 상태 관리
//    //private LaddersAnimator laddersAnimator; // 사다리 애니메이션 관리

//    public LaddersAction(LaddersState LaddersState)
//    {
//        laddersState = LaddersState; // 사다리 상태 초기화
//    }

//    public void Action()
//    {
//        int playerPositionY = Mathf.RoundToInt(laddersState.Player.transform.position.y); // 플레이어의 Y 좌표
//        int topPointY = Mathf.RoundToInt(laddersState.topPoint.transform.position.y); // 사다리의 위쪽 포인트 Y 좌표
//        int bottomPointY = Mathf.RoundToInt(laddersState.bottomPoint.transform.position.y); // 사다리의 아래쪽 포인트 Y 좌표

//        if(topPointY < bottomPointY)
//        {
//            // 위쪽 포인트가 아래쪽 포인트보다 낮은 경우, 위치를 교환
//            int temp = topPointY;
//            topPointY = bottomPointY;
//            bottomPointY = temp;
//        }

//        Debug.Log($"플레이어: {laddersState.Player.name}");
//        Debug.Log($"플레이어 Y 좌표: {playerPositionY}");
//        Debug.Log($"사다리 위쪽 포인트 Y 좌표: {topPointY}");
//        Debug.Log($"사다리 아래쪽 포인트 Y 좌표: {bottomPointY}");

//        // 플레이어가 사다리 위쪽에 있을 때
//        if (playerPositionY == topPointY)
//        {
//            // 아래쪽 포인트로 이동
//            laddersState.Player.transform.position = new Vector3(
//                laddersState.Player.transform.position.x,
//                laddersState.bottomPoint.transform.position.y,
//                laddersState.Player.transform.position.z
//            );
//            Debug.Log("아래로 이동");
//            Debug.Log($"사다리 이동: Y: {topPointY} to Y: {bottomPointY}");
//        }

//        // 플레이어가 사다리 아래쪽에 있을 때
//        else
//        {
//            // 위쪽 포인트로 이동
//            laddersState.Player.transform.position = new Vector3(
//                laddersState.Player.transform.position.x,
//                laddersState.topPoint.transform.position.y,
//                laddersState.Player.transform.position.z
//            );
//            Debug.Log("위로 이동");
//            Debug.Log($"사다리 이동: Y: {bottomPointY} to Y: {topPointY}");
//        }
//    }

//    public void Exit() { }

//    public void Setup()
//    {
//        // 사다리 타기 상태 초기화
//        Debug.Log("사다리 타기 상태를 초기화합니다.");
//    }
//}
