public class GimmickContext
{
    private IGimmickAction currentAction;

    public void SetAction(IGimmickAction action)
    {
        currentAction = action;
        currentAction?.setup();
    }

    public void ExecuteAction()
    {
        currentAction?.Action();
    }

    public void CancelAction()
    {
        currentAction?.Execute();
    }
}
