using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
        }
        else
        {
            GetComponent<Player>().SetupPlayer();
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();

        GameManager.RegisterPlayer(_netID, _player);
    }

    void OnDisable()
    {
        if (isLocalPlayer)
            GameManager.Instance.SetSceneCameraActive(true);

        GameManager.UnRegisterPlayer(transform.name);
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }
}