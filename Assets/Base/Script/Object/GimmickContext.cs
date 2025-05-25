public class GimmickContext
{
    private IGimmickAction currentAction;

    public void SetAction(IGimmickAction action)
    {
        currentAction = action;
        currentAction?.Setup();
    }

    public void StartAction()
    {
        currentAction?.Action();
    }

    public void CancelAction()
    {
        currentAction?.Exit();
    }
}
