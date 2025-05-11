using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerInteractionHandler : MonoBehaviour
{
    private BlockCollisionDetector collisionDetector;

    [SerializeField]
    private string blockTag = "Block"; // 상호작용 오브젝트의 태그

    [SerializeField]
    private bool ignoreCollision = true; // 충돌 무시 여부

    private Collider playerCollider;

    void Awake()
    {
        // BlockCollisionDetector 초기화
        collisionDetector = new BlockCollisionDetector(blockTag);
        collisionDetector.OnNewBlockDetected += HandleNewBlockDetected;

        // 플레이어의 Collider 가져오기
        playerCollider = GetComponent<Collider>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // BlockCollisionDetector로 충돌 처리
        collisionDetector.CheckCollision(collision);

        // 충돌 무시 설정
        if (ignoreCollision && collision.gameObject.CompareTag(blockTag))
        {
            Collider blockCollider = collision.collider;
            Physics.IgnoreCollision(playerCollider, blockCollider, true);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // BlockCollisionDetector로 충돌 종료 처리
        collisionDetector.ExitCollision(collision.transform, false);

        // 충돌 다시 활성화
        if (collision.gameObject.CompareTag(blockTag))
        {
            Collider blockCollider = collision.collider;
            Physics.IgnoreCollision(playerCollider, blockCollider, false);
        }
    }

    void HandleNewBlockDetected(Transform newBlock)
    {
        Debug.Log($"New block detected: {newBlock.name}");
        // 추가 로직을 여기에 구현 (예: 점수 증가, 효과 발생 등)
    }

    public void ResetCollisionState()
    {
        collisionDetector.Reset();
    }
}