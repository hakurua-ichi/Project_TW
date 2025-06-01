using UnityEngine;
using System;
using System.Collections.Generic;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();
    private static UnityMainThreadDispatcher _instance = null;
    private static bool _instanceExists = false;


    public static UnityMainThreadDispatcher Instance()
    {
        if (!_instanceExists)
        {
            lock (_executionQueue)
            {
                if (!_instanceExists)
                {
                    _instance = FindFirstObjectByType<UnityMainThreadDispatcher>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("[UnityMainThreadDispatcher]");
                        _instance = go.AddComponent<UnityMainThreadDispatcher>();
                        // DontDestroyOnLoad는 Awake에서 처리
                    }
                    _instanceExists = true; // 인스턴스 찾거나 생성 후 true로 설정
                }
            }
        }
        return _instance;
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            _instanceExists = true;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Enqueue(Action action)
    {
        if (action == null) return;
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    void Update()
    {
        if (_executionQueue.Count > 0)
        {
            lock (_executionQueue)
            {
                while (_executionQueue.Count > 0)
                {
                    try
                    {
                        _executionQueue.Dequeue().Invoke();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[UnityMainThreadDispatcher] Error executing action: {ex.ToString()}");
                    }
                }
            }
        }
    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
            _instanceExists = false;
        }
    }
}