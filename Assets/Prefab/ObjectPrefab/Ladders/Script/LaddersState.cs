using UnityEngine;

public class LaddersState : MonoBehaviour
{
    public bool useAble { get; set; } // 사다리 타기 상태
    public GameObject Player { get; private set; }
    public GameObject topPoint { get; private set; } // 첫 번째 사다리 포인트
    public GameObject bottomPoint { get; private set; } // 두 번째 사다리 포인트

    public void Set(GameObject thisPoint, GameObject otherPoint, GameObject player)
    {
        Player = otherPoint; // 플레이어 위치 설정
        topPoint = thisPoint; // 첫 번째 사다리 포인트 설정
        bottomPoint = otherPoint; // 두 번째 사다리 포인트 설정
    }
}
