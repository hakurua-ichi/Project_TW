using UnityEngine;

[RequireComponent(typeof(GimmickSubject))]
public class ElevatorGimmickRegister : MonoBehaviour
{
    private void Start()
    {
        GimmickSubject subject = GetComponent<GimmickSubject>();
        ElevatorController elevator = FindObjectOfType<ElevatorController>();

        if (elevator != null)
        {
            subject.AddObserverEnter(elevator);
        }
    }
}