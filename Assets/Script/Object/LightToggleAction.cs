using UnityEngine;

public class LightToggleAction : IGimmickAction
{
    private Light light;

    public LightToggleAction(Light light)
    {
        this.light = light;
    }

    public void Action()
    {
        Debug.Log("Light On");
        light.enabled = true;
    }

    public void Execute()
    {
        Debug.Log("Light off");
        light.enabled = false;
    }
}
