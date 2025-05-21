using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central publisher in the Observer pattern for gimmick interactions.
/// - Supports separate subscription lists for "enter" and "exit" style events.
/// - Broadcasts to *all* registered observers; no need to pass a specific caller.
/// - Keeps public API symmetrical and names self-explanatory.
/// </summary>
public class GimmickSubject : MonoBehaviour
{
    // ▶ Separate lists for clarity; could also be merged if both callbacks are always needed.
    private readonly List<IGimmickObserver> _enterObservers = new();
    private readonly List<IGimmickObserver> _exitObservers = new();

    #region Observer registration helpers

    public void AddEnterObserver(IGimmickObserver observer)
    {
        if (observer != null && !_enterObservers.Contains(observer))
            _enterObservers.Add(observer);
    }

    public void RemoveEnterObserver(IGimmickObserver observer) => _enterObservers.Remove(observer);

    public void AddExitObserver(IGimmickObserver observer)
    {
        if (observer != null && !_exitObservers.Contains(observer))
            _exitObservers.Add(observer);
    }

    public void RemoveExitObserver(IGimmickObserver observer) => _exitObservers.Remove(observer);

    #endregion

    #region Event dispatch

    /// <summary>
    /// Broadcast a "gimmick enter" event to every observer that registered via <see cref="AddEnterObserver"/>.
    /// Maps to <see cref="IGimmickObserver.OnGimmickTriggered"/>.
    /// </summary>
    public void NotifyEnter()
    {
        foreach (var obs in _enterObservers)
            obs?.OnGimmickTriggered();
    }

    /// <summary>
    /// Broadcast a "gimmick exit / button release" event to every observer that registered via <see cref="AddExitObserver"/>.
    /// Maps to <see cref="IGimmickObserver.ButtonClick"/>.
    /// </summary>
    public void NotifyExit()
    {
        foreach (var obs in _exitObservers)
            obs?.ButtonClick();
    }

    #endregion

    #region Backwards-compatibility shims (optional)
    // Note the discard parameter "_" to satisfy the compiler: a name is still required.
    [System.Obsolete("Use NotifyEnter() and maintain observers via AddEnterObserver instead.")]
    public void Notify(IGimmickObserver _) => NotifyEnter();

    [System.Obsolete("Use NotifyExit() and maintain observers via AddExitObserver instead.")]
    public void NotifyExit(IGimmickObserver _) => NotifyExit();
    #endregion
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