using UnityEngine;

public class FireToggleAction : IGimmickAction
{
    private ParticleSystem fire;

    public FireToggleAction(ParticleSystem fire)
    {
        this.fire = fire;
    }

    public void Action()
    {
        Debug.Log("Fire On");
        fire.Play();
    }

    public void Exit()
    {
        Debug.Log("Fire off");
        fire.Stop();
    }

    public void setup()
    {
        fire.Stop(); // 초기 상태를 꺼진 상태로 설정
    }
}
