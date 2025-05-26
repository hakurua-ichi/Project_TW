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
        foreach (SimplePlaneGroupController planeGroup in planeGroups)
        {
            // 각 그룹의 모든 Plane Collider 가져오기
            Collider[] planeColliders = planeGroup.GetAllPlaneColliders();
            
            foreach (Collider planeCollider in planeColliders)
            {
                // 카메라와 플레이어 사이에 plane이 있는지 체크
                RaycastHit hit;
                Vector3 direction = player.position - cameraTrans.position;
                float distance = direction.magnitude;
                
                if (Physics.Raycast(cameraTrans.position, direction.normalized, out hit, distance, wallLayers))
                {
                    // 히트된 오브젝트가 현재 plane인지 확인
                    if (hit.collider.gameObject == planeCollider.gameObject)
                    {
                        planeGroup.SetDetectedByWallDetector(planeCollider.gameObject, true);
                    }
                    else
                    {
                        planeGroup.SetDetectedByWallDetector(planeCollider.gameObject, false);
                    }
                }
                else
                {
                    planeGroup.SetDetectedByWallDetector(planeCollider.gameObject, false);
                }
            }
        }
    }
}