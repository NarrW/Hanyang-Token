using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;

public class GameScene : Scene
{
    void Awake()
    {
        
    }

	public virtual void Start ()
    {
#if UNITY_ANDROID && !UNITY_EDITOR

#else

#endif
    }

	public virtual void Update()
	{
	}
}
