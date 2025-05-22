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
        for (int i = list.Count - 1; i >= 0; i--)
        {
            var obs = list[i];
            if (obs == null) { list.RemoveAt(i); continue; } // 고스트 참조 청소
            call(obs);
        }
    }
}

#region old code
/*
구형 코드
using System.Collections.Generic;
using UnityEngine;

public class GimmickSubject : MonoBehaviour
{
    private List<IGimmickObserver> observersOn = new List<IGimmickObserver>();
    private List<IGimmickObserver> observersOff = new List<IGimmickObserver>();

    public void AddObserverEnter(IGimmickObserver observer)
    {
        if (!observersOn.Contains(observer))
            observersOn.Add(observer);
    }

    public void AddObserverExit(IGimmickObserver observer)
    {
        if (!observersOff.Contains(observer))
            observersOff.Add(observer);
    }

    public void RemoveObserverEnter(IGimmickObserver observer)
    {
        if (observersOn.Contains(observer))
            observersOn.Remove(observer);
    }

    public void RemoveObserverExit(IGimmickObserver observer)
    {
        if (observersOff.Contains(observer))
            observersOff.Remove(observer);
    }

    public void Notify(IGimmickObserver caller)
    {
        if (!observersOn.Contains(caller))
        {
            Debug.Log("해당 옵저버가 없습니다");
            return;  
        }

        Debug.Log("Notify called by: " + caller.GetType().Name);
        caller.OnGimmickTriggered();
        caller.ButtonClick();
        return;
    }

    public void NotifyExit(IGimmickObserver caller)
    {
        if (!observersOn.Contains(caller))
        {
            Debug.Log("해당 옵저버가 없습니다");
            return;  
        }

        Debug.Log("NotifyExit called by: " + caller.GetType().Name);
        caller.OnGimmickTriggered();
        caller.ButtonClick();
        return;
    }
}
*/
#endregion