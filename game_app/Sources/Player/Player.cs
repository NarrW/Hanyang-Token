using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    public BMSPlayer m_compBMSPlayer;

    [SyncVar(hook = "OnScoreChange")]
    public int score;
    private void OnScoreChange(int scoreChange)
    {
        score = scoreChange;
    }

    [Command]
    public void CmdProvideScoreToServer(int _score)
    {
        RpcTransmitScore(_score);

    }
    [ClientRpc]
    public void RpcTransmitScore(int _score)
    {
        score = _score;
    }



    [SyncVar(hook = "OnReadyChange")]
    public bool ready;
    private void OnReadyChange(bool _readyChange)
    {
        ready = _readyChange;
    }

    [Command]
    public void CmdProvideReadyToServer(bool _ready)
    {
        RpcTransmitReady(_ready);

    }
    [ClientRpc]
    public void RpcTransmitReady(bool _ready)
    {
        ready = _ready;
    }



    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    private bool firstSetup = true;

    public void SetupPlayer()
    {
        if (isLocalPlayer)
        {
            //Switch cameras
            GameManager.Instance.SetSceneCameraActive(false);
            //GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }

        CmdBroadCastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            firstSetup = false;
        }

        SetDefaults();
    }

    public void SetDefaults()
    {
        
    }
}
