using System.Collections.Generic;
using UnityEngine;

public class GimmickSubject : MonoBehaviour
{
    private List<IGimmickObserver> observers = new List<IGimmickObserver>();

    public void AddObserver(IGimmickObserver observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);
    }

    public void RemoveObserver(IGimmickObserver observer)
    {
        if (observers.Contains(observer))
            observers.Remove(observer);
    }

    public void Notify()
    {
        Debug.Log("Notify ½ÇÇà");
        foreach (var observer in observers)
        {
            Debug.Log("Notifying observer: " + observer.GetType().Name);
            observer.OnGimmickTriggered();
        }
    }
}
