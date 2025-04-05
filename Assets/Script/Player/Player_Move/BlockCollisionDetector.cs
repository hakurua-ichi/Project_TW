using UnityEngine;
using System;

public class BlockCollisionDetector
{
    private string blockTag;
    public Transform CurrentBlock { get; private set; }
    public bool IsGrounded { get; private set; }
    
    // 블록 감지 이벤트
    public event Action<Transform> OnNewBlockDetected;
    
    public BlockCollisionDetector(string tag)
    {
        blockTag = tag;
        CurrentBlock = null;
        IsGrounded = false;
    }
    
    public void CheckCollision(Collision collision)
    {
        // 태그로 블록 확인
        if (collision == null || !collision.gameObject.CompareTag(blockTag)) return;
        
        bool foundValidContact = false;
        float highestContactY = float.MinValue;
        
        // 모든 접점 검사하여 가장 위에 있는 접점 찾기
        foreach (ContactPoint contact in collision.contacts)
        {
            // Y축 노말이 0.7 이상이면 위에서 밟고 있다고 판단
            if (contact.normal.y > 0.7f && contact.point.y > highestContactY)
            {
                highestContactY = contact.point.y;
                foundValidContact = true;
            }
        }
        
        if (foundValidContact)
        {
            IsGrounded = true;
            
            // 새 블록 감지 시
            if (CurrentBlock != collision.transform)
            {
                CurrentBlock = collision.transform;
                OnNewBlockDetected?.Invoke(CurrentBlock);
            }
        }
    }
    
    public void ExitCollision(Transform collidedTransform, bool isRotating)
    {
        // 맵이 회전 중이 아닐 때만 블록 참조 초기화
        if (CurrentBlock == collidedTransform && !isRotating)
        {
            CurrentBlock = null;
            IsGrounded = false;
        }
    }
    
    public void Reset()
    {
        CurrentBlock = null;
        IsGrounded = false;
    }
}