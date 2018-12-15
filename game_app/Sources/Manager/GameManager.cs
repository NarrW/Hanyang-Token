using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

public class GameManager : MonoBehaviour
{
    private static GameManager m_Instance;
    public static GameManager Instance  
	{  
		get
		{
			if( m_Instance == null )  
			{
                m_Instance = FindObjectOfType(typeof(GameManager)) as GameManager;
				if( m_Instance == null ) 
				{
					GameObject container = new GameObject();  
					container.name = "GameManager";
                    m_Instance = container.AddComponent(typeof(GameManager)) as GameManager;  
				}
			}
			return m_Instance;
		}
	}

    [SerializeField]
    private GameObject sceneCamera;

    void Awake()
	{
		DontDestroyOnLoad (this);

        UnityEngine.Debug.Log("GameManager Awake");

        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 120;
    }
	
	void OnApplicationQuit()
	{
		m_Instance = null;

        BassManager.Instance.Release();
	}

    private bool bIsInit = false;

    public void Init()
	{
        if (bIsInit == false)
        {
            BassManager.Instance.Init();

            bIsInit = true;
        }
    }

    public BMSLoader m_cCurrentPlayingBMS;

    public void SetSceneCameraActive(bool isActive)
    {
        if (sceneCamera == null)
            return;

        sceneCamera.SetActive(isActive);
    }

    private const string PLAYER_ID_PREFIX = "Player ";

    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public static void RegisterPlayer(string _netID, Player _player)
    {
        string _playerID = PLAYER_ID_PREFIX + _netID;
        players.Add(_playerID, _player);
        _player.transform.name = _playerID;
    }

    public static void UnRegisterPlayer(string _playerID)
    {
        players.Remove(_playerID);
    }

    public static Player GetPlayer(string _playerID)
    {
        return players[_playerID];
    }

    public static Player[] GetAllPlayers()
    {
        return players.Values.ToArray();
    }

    public static int GetPlayerAmount()
    {
        return players.Count;
    }

    public static bool IsAllPlayerLoadingFinished()
    {
        foreach (Player p in players.Values)
        {
            if (p.ready == false)
            {
                return false;
            }
        }

        return true;
    }
}
