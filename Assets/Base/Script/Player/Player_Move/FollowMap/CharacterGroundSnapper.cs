using UnityEngine;
    /// <summary>
    /// CharacterGroundSnapper
    /// 
    /// 캐릭터를 항상 지면에 붙여있게 하는 컴포넌트
    /// 회전 맵 위에서 캐릭터가 공중에 뜨는 문제를 해결하기 위해 사용
    /// 
    /// 주요 기능:
    /// - 레이캐스트를 통한 지면 감지
    /// - 일정 거리 이상 떨어진 경우 지면으로 스냅
    /// - 캐릭터 공중 부양 방지
    /// </summary>
    public class CharacterGroundSnapper : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private float raycastDistance = 2.0f;
    [SerializeField] private float snapThreshold = 0.5f;
    [SerializeField] private LayerMask groundLayers = -1; // 기본값: 모든 레이어

    [Header("디버그")]
    [SerializeField] private bool showDebugRays = false;

    private CharacterController controller;
    private Vector3 lastGroundPosition;
    private float characterHeight;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("CharacterGroundSnapper 컴포넌트는 CharacterController가 필요합니다!");
            enabled = false;
            return;
        }

        // 캐릭터의 높이 저장
        characterHeight = controller.height;
    }

    private void LateUpdate()
    {
        // 이미 지면에 있으면 스냅핑 필요 없음
        if (controller.isGrounded)
        {
            lastGroundPosition = transform.position;
            return;
        }

        // 발사할 광선의 시작점 (캐릭터의 중심에서 약간 위)
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        
        // 아래 방향으로 광선 발사
        RaycastHit hit;
        if (Physics.Raycast(rayStart, Vector3.down, out hit, raycastDistance, groundLayers))
        {
            // 디버그 광선 표시
            if (showDebugRays)
            {
                Debug.DrawRay(rayStart, Vector3.down * raycastDistance, Color.green);
            }

            // 지면과의 거리 계산 (캐릭터 크기 고려)
            float distanceToGround = hit.distance - 0.1f; // 시작점이 0.1f 위에서 시작하므로 보정
            
            // 일정 거리 이상 떨어진 경우에만 스냅
            if (distanceToGround <= snapThreshold && !controller.isGrounded)
            {
                // 위치 보정 - CharacterController.Move() 사용하여 이동
                Vector3 adjustment = Vector3.down * distanceToGround;
                controller.Move(adjustment);
                
                // 디버그 로그
                if (showDebugRays)
                {
                    Debug.Log($"캐릭터 지면에 스냅: {distanceToGround}m 보정");
                }            }
        }
        else if (showDebugRays)
        {
            // 지면을 찾지 못한 경우 디버그 레이 표시
            Debug.DrawRay(rayStart, Vector3.down * raycastDistance, Color.red);
        }
    }
}
