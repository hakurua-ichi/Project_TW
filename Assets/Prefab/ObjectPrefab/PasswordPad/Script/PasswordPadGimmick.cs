using UnityEngine;

public class PasswordPadGimmick : MonoBehaviour, IGimmickObserver
{
    [Header("▶ 이 Pad의 인덱스 (0부터 시작)")]
    [SerializeField] private int padIndex = 0;

    [Header("▶ 화면에 숫자를 보여줄 StateText (Optional)")]
    [SerializeField] private StateText StateText;

    [Header("▶ 씬에 단 하나만 있어야 하는 PasswordData")]
    [SerializeField] private PasswordData PasswordData;

    private GimmickContext context;
    private PasswordPadAction actionScript;

    void Awake()
    {
        if (PasswordData == null)
        {
            Debug.LogError("PasswordPadGimmick: Inspector에서 PasswordData를 연결해야 합니다.");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        // 이 GameObject에 PasswordPadAction을 붙이고 초기화
        actionScript = gameObject.AddComponent<PasswordPadAction>();
        actionScript.Initialize(padIndex, StateText, PasswordData);

        // GimmickContext 생성
        context = new GimmickContext();
        context.SetAction(actionScript);
    }

    public void OnGimmickEnter() { }

    public void OnGimmickLeave() { }

    /// <summary>
    /// 버튼 클릭 시 이 메서드를 호출하게 하세요(Inspector의 UI Button OnClick 등).
    /// </summary>
    public void ButtonClick()
    {
        context.StartAction();
    }
}
