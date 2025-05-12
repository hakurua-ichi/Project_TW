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

    public void Notify()
    {
        Debug.Log("可历滚 角青");
        foreach (var observer in observersOn)
        {
            Debug.Log("Notifying observer: " + observer.GetType().Name);
            observer.OnGimmickTriggered();
            observer.ButtonClick();
        }
    }

    public void NotifyExit()
    {
        Debug.Log("Exit 可历滚 角青");
        foreach (var observer in observersOff)
        {
            Debug.Log("Notifying observer: " + observer.GetType().Name);
            observer.OnGimmickTriggered();
            observer.ButtonClick();
        }
    }
}
