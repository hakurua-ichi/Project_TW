using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 투명 영역 내 벽 감지
/// </summary>
public class WallDetector
{
    private LayerMask wallLayers;
    private SimplePlaneGroupController[] planeGroups;

    public WallDetector(LayerMask wallLayers)
    {
        this.wallLayers = wallLayers;
        planeGroups = Object.FindObjectsOfType<SimplePlaneGroupController>();
    }

    public Collider[] DetectWalls(GameObject transparentZone)
    {
        if (!transparentZone) return new Collider[0];

        return Physics.OverlapBox(
            transparentZone.transform.position,
            transparentZone.transform.lossyScale / 2f,
            transparentZone.transform.rotation,
            wallLayers
        );
    }

    public void DetectPlanes(Transform player, Transform cameraTrans)
    {
        // null 체크
        if (planeGroups == null || planeGroups.Length == 0)
        {
            // 재시도: SimplePlaneGroupController 찾기
            planeGroups = Object.FindObjectsByType<SimplePlaneGroupController>(FindObjectsSortMode.None);
            
            if (planeGroups == null || planeGroups.Length == 0)
            {
                Debug.LogWarning("WallDetector: SimplePlaneGroupController를 찾을 수 없습니다.");
                return;
            }
            
            Debug.Log($"WallDetector: {planeGroups.Length}개의 SimplePlaneGroupController를 찾았습니다.");
        }
        
        // 평면 감지 강화
        foreach (SimplePlaneGroupController planeGroup in planeGroups)
        {
            if (planeGroup == null) continue;
            
            Collider[] planeColliders = planeGroup.GetAllPlaneColliders();
            if (planeColliders == null || planeColliders.Length == 0) continue;
            
            foreach (Collider planeCollider in planeColliders)
            {
                if (planeCollider == null) continue;
                
                // 카메라와 플레이어 사이에 plane이 있는지 체크
                Vector3 direction = player.position - cameraTrans.position;
                float distance = direction.magnitude;
                
                // 레이캐스트 두 번 실행 (모든 레이어, 그리고 플레인의 레이어만)
                RaycastHit hit;
                bool isDetected = false;
                
                // 1. 모든 레이어 대상 체크
                if (Physics.Raycast(cameraTrans.position, direction.normalized, out hit, distance, -1))
                {
                    if (hit.collider.gameObject == planeCollider.gameObject)
                    {
                        isDetected = true;
                        Debug.DrawLine(cameraTrans.position, hit.point, Color.green, 0.1f);
                    }
                }
                
                // 2. 플레인 레이어만 체크 (더 확실한 감지)
                if (!isDetected)
                {
                    int planeLayer = 1 << planeCollider.gameObject.layer;
                    if (Physics.Raycast(cameraTrans.position, direction.normalized, out hit, distance, planeLayer))
                    {
                        if (hit.collider.gameObject == planeCollider.gameObject)
                        {
                            isDetected = true;
                            Debug.DrawLine(cameraTrans.position, hit.point, Color.blue, 0.1f);
                        }
                    }
                }
                
                // 감지 상태 설정 (투명화 적용)
                planeGroup.SetDetectedByWallDetector(planeCollider.gameObject, isDetected);
                
                // 디버그 메시지
                if (isDetected)
                {
                    Debug.Log($"Plane 감지됨: {planeCollider.gameObject.name}, 거리: {hit.distance}");
                }
            }
        }
    }
}