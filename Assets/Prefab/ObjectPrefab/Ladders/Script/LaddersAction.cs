using System.Collections;
using UnityEngine;

/// <summary> 사다리 탑승-이동 액션 </summary>
public class LaddersAction : IGimmickAction
{
    private readonly LaddersState laddersState;
    private readonly MonoBehaviour runner;             // 코루틴 실행자

    public LaddersAction(LaddersState state, MonoBehaviour coroutineRunner)
    {
        laddersState = state;
        runner = coroutineRunner;
    }

    /* ---------------------------------------------------------------- */
    /* IGimmickAction 구현                                              */
    /* ---------------------------------------------------------------- */

    public void Setup() { }
    public void Exit() { }

    public void Action()
    {
        if (laddersState.Player == null) return;

        float topY = laddersState.topPoint.transform.position.y;
        float bottomY = laddersState.bottomPoint.transform.position.y;
        float highY = Mathf.Max(topY, bottomY);
        float lowY = Mathf.Min(topY, bottomY);
        float pY = laddersState.Player.transform.position.y;

        if (pY >= highY - 0.1f)
            runner.StartCoroutine(TeleportWithExit(laddersState.Player, lowY));
        else
            runner.StartCoroutine(TeleportWithExit(laddersState.Player, highY));
    }

    /* ---------------------------------------------------------------- */
    /* 내부 코루틴                                                      */
    /* ---------------------------------------------------------------- */

    private IEnumerator TeleportWithExit(GameObject playerObj, float targetY)
    {
        var cc = playerObj.GetComponent<CharacterController>();
        var moveCtrl = playerObj.GetComponent<PlayerMovementController>();

        // ── ① 이동 스크립트 & 콜라이더 OFF ───────────────────────────────
        bool moverWasOn = moveCtrl ? moveCtrl.enabled : true;
        if (moveCtrl) moveCtrl.enabled = false;
        if (cc) cc.enabled = false;

        // ── ② 위치 순간 이동 + 물리 강제 동기화 ────────────────────────
        playerObj.transform.position = new Vector3(
            playerObj.transform.position.x,
            targetY,
            playerObj.transform.position.z);
        Physics.SyncTransforms();

        // ── ③ 물리 사이클 1-2회 기다려 Exit 이벤트 발생 보장 ─────────
        yield return new WaitForFixedUpdate();   // 1번째 FixedUpdate
        yield return new WaitForFixedUpdate();   // 2번째 FixedUpdate

        // ── ④ inRange/버튼 상태 재계산 ────────────────────────────────
        ProximityTriggerObject.NotifyPlayerTeleported();  // <- 새 메서드(아래) 추가
        InteractionsButtonAction.Instance?.RequestSelection(null, null);

        // ── ⑤ 스크립트 & 콜라이더 복구 ───────────────────────────────
        if (cc) cc.enabled = true;
        if (moveCtrl) moveCtrl.enabled = moverWasOn;
    }

}




//using UnityEngine;

//public class LaddersAction : IGimmickAction
//{
//    private LaddersState laddersState;

//    public LaddersAction(LaddersState state)
//    {
//        laddersState = state;
//    }

//    public void Setup()
//    {
//        Debug.Log("사다리 탑승 준비 완료");
//    }

//    public void Exit() { }

//    public void Action()
//    {
//        if (laddersState.Player == null ||
//            laddersState.topPoint == null ||
//            laddersState.bottomPoint == null)
//        {
//            Debug.LogWarning("LaddersAction: Player 또는 포인트가 null입니다.");
//            return;
//        }

//        int playerPositionY = Mathf.RoundToInt(laddersState.Player.transform.position.y); // 플레이어의 Y 좌표
//        int topPointY = Mathf.RoundToInt(laddersState.topPoint.transform.position.y); // 사다리의 위쪽 포인트 Y 좌표
//        int bottomPointY = Mathf.RoundToInt(laddersState.bottomPoint.transform.position.y); // 사다리의 아래쪽 포인트 Y 좌표

//        float highY = Mathf.Max(topPointY, bottomPointY);
//        float lowY = Mathf.Min(topPointY, bottomPointY);

//        // 위쪽에 있으면 아래로, 아래쪽에 있으면 위로
//        if (playerPositionY >= highY - 0.1f)
//        {
//            MovePlayerToY(laddersState.Player, lowY);
//            Debug.Log($"아래로 이동: {playerPositionY:F2} → {lowY:F2}");
//        }
//        else if (playerPositionY <= lowY + 0.1f)
//        {
//            MovePlayerToY(laddersState.Player, highY);
//            Debug.Log($"위로 이동: {playerPositionY:F2} → {highY:F2}");
//        }
//        else
//        {
//            Debug.LogWarning($"사다리 중간에서 Action() 호출됨 (playerY={playerPositionY:F2})");
//        }
//    }

//    /// <summary>
//    /// CharacterController 또는 Rigidbody를 고려해서
//    /// 플레이어를 targetY로 안전하게 이동시키는 헬퍼 메서드입니다.
//    /// </summary>
//    private void MovePlayerToY(GameObject playerObj, float targetY)
//    {
//        // 1) PlayerObj 또는 자식에서 CharacterController 찾기
//        CharacterController cc = playerObj.GetComponent<CharacterController>();
//        if (cc == null)
//            cc = playerObj.GetComponentInChildren<CharacterController>();

//        // 2) 이동 스크립트(예: PlayerMovement)가 있다면 잠시 꺼두기
//        var customMover = playerObj.GetComponent<CharacterController>();
//        //if (customMover != null) customMover.enabled = false;

//        if (cc != null)
//        {
//            // CharacterController가 붙어 있으면 잠깐 비활성화 후 이동
//            cc.enabled = false;
//            playerObj.transform.position = new Vector3(
//                playerObj.transform.position.x,
//                targetY,
//                playerObj.transform.position.z
//            );
//            Debug.Log($"(사다리) 이동 직후 Y: {playerObj.transform.position.y:F2}");
//            cc.enabled = true;
//        }
//        else
//        {
//            // Rigidbody가 붙어 있고 isKinematic=false라면 잠깐 Kinematic으로 전환 후 이동
//            Rigidbody rb = playerObj.GetComponent<Rigidbody>();
//            if (rb != null && !rb.isKinematic)
//            {
//                //rb.isKinematic = true;
//                playerObj.transform.position = new Vector3(
//                    playerObj.transform.position.x,
//                    targetY,
//                    playerObj.transform.position.z
//                );
//                Debug.Log($"(사다리) 이동 직후 Y: {playerObj.transform.position.y:F2}");
//                //rb.isKinematic = false;
//            }
//            else
//            {
//                // 그 외에는 그냥 Transform 위치만 이동
//                playerObj.transform.position = new Vector3(
//                    playerObj.transform.position.x,
//                    targetY,
//                    playerObj.transform.position.z
//                );
//                Debug.Log($"(사다리) 이동 직후 Y: {playerObj.transform.position.y:F2}");
//            }
//        }

//        // 다시 이동 스크립트 켜기
//        //if (customMover != null) customMover.enabled = true;
//    }
//}
