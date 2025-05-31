using UnityEngine;

public class LaddersState
{
    public bool useAble { get; set; } = false; // 사다리 타기 상태
    public GameObject Player { get; private set; } = null; // 플레이어 오브젝트
    public GameObject topPoint { get; private set; } = null; // 첫 번째 사다리 포인트
    public GameObject bottomPoint { get; private set; } = null; // 두 번째 사다리 포인트

    public void Set(GameObject thisPoint, GameObject otherPoint, GameObject player)
    {
        Player = player; // 플레이어 위치 설정
        topPoint = thisPoint; // 첫 번째 사다리 포인트 설정
        bottomPoint = otherPoint; // 두 번째 사다리 포인트 설정
    }
}
