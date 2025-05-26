using UnityEngine;

public class KeyGimmick : MonoBehaviour, IGimmickObserver
{
    private GimmickContext context;
    private Inventory inventory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        context = new GimmickContext();
    }
    
    public void OnGimmickEnter() { }
    public void OnGimmickLeave() { }
    public void ButtonClick()
    {
        Debug.Log("버튼 클릭됨");
        if (inventory.hasItem)
        {
            Debug.Log("인벤토리 가득 참");
            return;
        }
        else
        {
            context.StartAction();
            inventory.hasItem = true;
            Destroy(gameObject);
        }
    }
}
