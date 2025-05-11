using UnityEngine;

/// <summary>
/// 투명 영역 내 벽 감지
/// </summary>
public class WallDetector
{
    private LayerMask wallLayers;

    public WallDetector(LayerMask wallLayers)
    {
        this.wallLayers = wallLayers;
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
}