using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SingleTonTestScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pienna_TestScene_nextScene()
    {
        SceneManager.LoadScene("pienna_TestScene 1");
    }

    public void pienna_TestScene_1_nextScene()
    {
        SceneManager.LoadScene("pienna_TestScene");
    }
}
