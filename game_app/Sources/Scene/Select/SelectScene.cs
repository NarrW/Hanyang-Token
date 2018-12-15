using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;

public class SelectScene : Scene
{
    public Video compVideo;

    void Awake()
    {
        // 기본 세팅
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 120;
        UnityEngine.Debug.Log("GameManager Awake");

        GameManager.Instance.Init();
        ParticleManager.Instance.Init();
        DataManager.Instance.Init();
    }

    void Start()
    {
        
    }

    void Update()
    {
    }
}
