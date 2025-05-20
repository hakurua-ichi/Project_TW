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
