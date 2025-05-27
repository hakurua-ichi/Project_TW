using UnityEngine;

/// <summary>
/// 플레이어의 이동을 제한하는 기능을 제공하는 유틸리티 클래스
/// 맵 회전, 카메라 회전 및 기타 기믹 상황에서 플레이어의 이동을 제한
/// 싱글톤 패턴으로 구현되어 전역적으로 접근 가능
/// </summary>
public class PlayerMovementRestrictor : MonoBehaviour
{
    // 싱글톤 인스턴스
    private static PlayerMovementRestrictor _instance;
    public static PlayerMovementRestrictor Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에서 찾기
                _instance = FindObjectOfType<PlayerMovementRestrictor>();
                
                // 없으면 새로 생성
                if (_instance == null)
                {
                    GameObject obj = new GameObject("PlayerMovementRestrictor");
                    _instance = obj.AddComponent<PlayerMovementRestrictor>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }
    
    // 이동 제한 이유 열거형
    public enum RestrictionReason
    {
        None,
        MapRotation,
        CameraRotation,
        Cutscene,
        Dialog,
        Gimmick,
        Other
    }
    
    // 현재 활성화된 제한 이유들
    private bool[] activeRestrictions;
    
    private void Awake()
    {
        // 싱글톤 설정
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        // 제한 배열 초기화
        int restrictionCount = System.Enum.GetValues(typeof(RestrictionReason)).Length;
        activeRestrictions = new bool[restrictionCount];
    }
    
    /// <summary>
    /// 특정 이유로 플레이어의 이동을 제한
    /// </summary>
    /// <param name="reason">이동 제한 이유</param>
    public void RestrictMovement(RestrictionReason reason)
    {
        if (reason == RestrictionReason.None) return;
        
        // 제한 설정
        activeRestrictions[(int)reason] = true;
        
        // 플레이어 이동 업데이트
        UpdatePlayerMovement();
    }
    
    /// <summary>
    /// 특정 이유로 인한 이동 제한을 해제
    /// </summary>
    /// <param name="reason">해제할 이동 제한 이유</param>
    public void AllowMovement(RestrictionReason reason)
    {
        if (reason == RestrictionReason.None) return;
        
        // 제한 해제
        activeRestrictions[(int)reason] = false;
        
        // 플레이어 이동 업데이트
        UpdatePlayerMovement();
    }
    
    /// <summary>
    /// 특정 이유로 인한 이동 제한이 있는지 확인
    /// </summary>
    /// <param name="reason">확인할 이동 제한 이유</param>
    /// <returns>해당 이유로 이동이 제한되어 있으면 true, 그렇지 않으면 false</returns>
    public bool IsMovementRestricted(RestrictionReason reason)
    {
        if (reason == RestrictionReason.None) return false;
        return activeRestrictions[(int)reason];
    }
    
    /// <summary>
    /// 현재 이동이 제한되어 있는지 확인
    /// </summary>
    /// <returns>이동 제한 이유가 하나라도 있으면 true, 그렇지 않으면 false</returns>
    public bool IsMovementRestricted()
    {
        for (int i = 1; i < activeRestrictions.Length; i++)
        {
            if (activeRestrictions[i]) return true;
        }
        return false;
    }
    
    // 플레이어 이동 상태 업데이트
    private void UpdatePlayerMovement()
    {
        PlayerMovementController[] controllers = FindObjectsByType<PlayerMovementController>(FindObjectsSortMode.None);
        bool canMove = !IsMovementRestricted();
        
        foreach (PlayerMovementController controller in controllers)
        {
            controller.SetMovementEnabled(canMove);
            
            // 애니메이션 상태도 업데이트
            PlayerAnimationController animController = controller.GetComponent<PlayerAnimationController>();
            if (animController != null && !canMove)
            {
                animController.UpdateAnimationState(false);
            }
        }
    }
}
