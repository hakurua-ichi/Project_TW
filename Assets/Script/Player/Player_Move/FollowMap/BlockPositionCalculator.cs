using UnityEngine;

public class BlockPositionCalculator
{
    public Vector3 CalculatePositionAboveBlock(Transform block)
    {
        if (block == null) return Vector3.zero;
        
        // 블록 중앙 위치 계산
        Vector3 blockCenter = block.position;
        
        // 블록 상단으로 이동
        return new Vector3(
            blockCenter.x, 
            CalculateBlockTopPosition(block),
            blockCenter.z
        );
    }
    
    public float CalculateBlockTopPosition(Transform block)
    {
        if (block == null) return 0f;
        
        Vector3 blockCenter = block.position;
        float blockHeight = 1.0f;
        float topYPosition;
        
        // 콜라이더로 높이 계산
        Collider blockCollider = block.GetComponent<Collider>();
        if (blockCollider != null)
        {
            blockHeight = blockCollider.bounds.size.y;
            return blockCollider.bounds.center.y + blockHeight / 2;
        }
        
        // 렌더러로 높이 계산
        Renderer blockRenderer = block.GetComponent<Renderer>();
        if (blockRenderer != null)
        {
            blockHeight = blockRenderer.bounds.size.y;
            return blockRenderer.bounds.center.y + blockHeight / 2;
        }
        
        // 기본 계산
        blockHeight = block.localScale.y;
        return blockCenter.y + blockHeight / 2;
    }
}