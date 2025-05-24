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

    public void Exit()
    {
        Debug.Log("Light off");
        light.enabled = false;
    }

    public void Setup()
    {
        light.enabled = false; // 초기 상태를 꺼진 상태로 설정
    }
}
