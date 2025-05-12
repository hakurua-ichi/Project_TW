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
        Debug.Log("Notify called by: " + caller.GetType().Name);
        if (observersOn.Contains(caller))
        {
            for(int i = 0; i < observersOn.Count; i++)
            {
                if (observersOn[i] == caller)
                {
                    Debug.Log("Notify called by: " + caller.GetType().Name);
                    observersOn[i].OnGimmickTriggered();
                    observersOn[i].ButtonClick();
                    return;
                }
                else
                {
                    Debug.Log("해당 옵저버가 없습니다");
                    return;
                }
            }
        }
    }

    public void NotifyExit(IGimmickObserver caller)
    {
        for (int i = 0; i < observersOn.Count; i++)
        {
            if (observersOn[i] == caller)
            {
                Debug.Log("NotifyExit called by: " + caller.GetType().Name);
                observersOn[i].OnGimmickTriggered();
                observersOn[i].ButtonClick();
                return;
            }
            else
            {
                Debug.Log("해당 옵저버가 없습니다");
                return;
            }
        }
    }
}
