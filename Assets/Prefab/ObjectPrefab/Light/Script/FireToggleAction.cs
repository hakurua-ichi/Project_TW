using UnityEngine;

public class FireToggleAction : IGimmickAction
{
    private ParticleSystem fire;
    private AudioSource fireAudio;

    public FireToggleAction(ParticleSystem fire, AudioSource fireAudio)
    {
        this.fire = fire;
        this.fireAudio = fireAudio;
    }

    public void Action()
    {
        Debug.Log("Fire On");
        fire.Play();
        fireAudio.Play();
    }

    public void Exit()
    {
        Debug.Log("Fire off");
        fire.Stop();
        fireAudio.Stop();
    }

    public void Setup()
    {
        fire.Stop(); // 초기 상태를 꺼진 상태로 설정
        fireAudio.Stop(); // 초기 상태를 꺼진 상태로 설정
    }
}
