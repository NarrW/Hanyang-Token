using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;

public class MainScene : Scene
{
    public Video compVideo;

    void Awake()
    {
        // 기본 세팅
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 120;
        //QualitySettings.vSyncCount = 2;
        //QualitySettings.antiAliasing = 4;
        UnityEngine.Debug.Log("GameManager Awake");
		
        GameManager.Instance.Init();
        ParticleManager.Instance.Init();
    }

	public virtual void Start ()
    {
        Camera.main.aspect = 1920.0f / 1080.0f;

#if UNITY_ANDROID && !UNITY_EDITOR

#else

#endif
    }

	public virtual void Update()
	{
	}
}
