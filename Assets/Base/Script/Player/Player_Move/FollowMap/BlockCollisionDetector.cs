using UnityEngine;
using System;

public class BlockCollisionDetector
{
    private string blockTag;
    private Transform currentBlock;
    
    public Transform CurrentBlock => currentBlock;
    public event System.Action<Transform> OnNewBlockDetected;
    
    public BlockCollisionDetector(string tag)
    {
        blockTag = tag;
    }
    
    public void CheckCollision(Collision collision)
    {
        // 태그 체크와 함께 부모 계층에서도 RotateMap 검색
        GameObject collidedObject = collision.gameObject;
        bool hasMatchingTag = collidedObject.CompareTag(blockTag);
        bool hasRotateMapInParent = collidedObject.GetComponentInParent<RotateMap>() != null;
        
        if (hasMatchingTag || hasRotateMapInParent)
        {
            if (currentBlock == null || currentBlock != collision.transform)
            {
                currentBlock = collision.transform;
                OnNewBlockDetected?.Invoke(currentBlock);
            }
        }
    }
    
    public void ExitCollision(Transform collidedTransform, bool isRotating)
    {
        // 맵이 회전 중이 아닐 때만 블록 참조 초기화
        if (currentBlock == collidedTransform && !isRotating)
        {
            currentBlock = null;
        }
    }
    
    public void Reset()
    {
        currentBlock = null;
    }
}