using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central publisher in the Observer pattern for gimmick interactions.
/// - Three independent event channels: Enter / Leave / Button.
/// - Prevents duplicate registration, cleans up null-entries on dispatch.
/// </summary>
public class GimmickSubject : MonoBehaviour
{
    private readonly List<IGimmickObserver> _enterObservers = new();
    private readonly List<IGimmickObserver> _leaveObservers = new();
    private readonly List<IGimmickObserver> _buttonObservers = new();

    /* ────────── 공통 헬퍼 ────────── */
    private static void TryAdd(List<IGimmickObserver> list, IGimmickObserver o)
    { if (o != null && !list.Contains(o)) list.Add(o); }

    private static void TryRemove(List<IGimmickObserver> list, IGimmickObserver o)
    { if (o != null) list.Remove(o); }

    /* ────────── 등록 / 해제 ────────── */
    public void AddEnterObserver(IGimmickObserver o) => TryAdd(_enterObservers, o);
    public void RemoveEnterObserver(IGimmickObserver o) => TryRemove(_enterObservers, o);

    public void AddLeaveObserver(IGimmickObserver o) => TryAdd(_leaveObservers, o);
    public void RemoveLeaveObserver(IGimmickObserver o) => TryRemove(_leaveObservers, o);

    public void AddButtonObserver(IGimmickObserver o) => TryAdd(_buttonObservers, o);
    public void RemoveButtonObserver(IGimmickObserver o) => TryRemove(_buttonObservers, o);

    /* ────────── 이벤트 브로드캐스트 ────────── */
    public void NotifyEnter() => Dispatch(_enterObservers, obs => obs.OnGimmickEnter());
    public void NotifyLeave() => Dispatch(_leaveObservers, obs => obs.OnGimmickLeave());
    public void NotifyButton() => Dispatch(_buttonObservers, obs => obs.ButtonClick());

    /* ────────── 내부: null-세이프 디스패처 ────────── */
    private static void Dispatch(List<IGimmickObserver> list, System.Action<IGimmickObserver> call)
    {
        Debug.Log("디스패치 실행");
        for (int i = list.Count - 1; i >= 0; i--)
        {
            var obs = list[i];
            if (obs == null) { list.RemoveAt(i); continue; } // 고스트 참조 청소
            call(obs);
        }
    }
}
