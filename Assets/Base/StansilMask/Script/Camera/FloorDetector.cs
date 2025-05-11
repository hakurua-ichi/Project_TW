using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 바닥 감지 및 높이 계산
/// </summary>
public class FloorDetector
{
    private Transform player;
    private Transform cameraTransform;
    private LayerMask wallLayers;

    public FloorDetector(Transform player, Transform cameraTransform, LayerMask wallLayers)
    {
        this.player = player;
        this.cameraTransform = cameraTransform;
        this.wallLayers = wallLayers;
    }

    public float CalculateFloorEdgeDistance()
    {
        // null 체크 추가
        if (player == null || cameraTransform == null)
        {
            Debug.LogError("FloorDetector: player 또는 cameraTransform이 null입니다.");
            return 5f; // 기본값 반환
        }

        float minDistance = 5f;
        
        // 더 다양한 방향으로 바닥 감지 시도
        Vector3[] directions = { 
            Vector3.down, 
            -cameraTransform.up, 
            -player.up, 
            (Vector3.down - cameraTransform.up).normalized // 대각선 방향 추가
        };

        foreach (Vector3 dir in directions)
        {
            if (Physics.Raycast(player.position, dir, out RaycastHit hit, 5f, wallLayers))
            {
                // AABB 방식으로 가장 가까운 점 찾기
                Vector3 closestPoint = hit.collider.bounds.ClosestPoint(cameraTransform.position);
                float dist = Vector3.Distance(cameraTransform.position, closestPoint);
                
                minDistance = Mathf.Min(minDistance, dist);
                
                // 디버그 정보
                //Debug.DrawLine(player.position, hit.point, Color.red, 0.1f);
                //Debug.Log($"바닥 감지: 거리={dist}, 방향={dir}");
            }
        }

        return minDistance;
    }

    public float CalculateRequiredZoneHeight()
    {
        if (Physics.Raycast(player.position, Vector3.up, out RaycastHit hit, 10f, wallLayers))
            return hit.distance + 1.0f;

        return Vector3.Distance(cameraTransform.position, player.position) * 2f;
    }

    public List<GameObject> GetExcludedFloorObjects()
    {
        List<GameObject> excluded = new List<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(player.position, 1f, wallLayers);

        foreach (Collider col in colliders)
        {
            if (col.bounds.max.y <= player.position.y + 0.1f)
                excluded.Add(col.gameObject);
        }

        return excluded;
    }
}