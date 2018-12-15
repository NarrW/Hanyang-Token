using UnityEngine;
using System.Collections;

public class LineEffect : MonoBehaviour
{
    private float m_fNoteSpeed;
    private float m_fScrollAmount;

    public Vector3 m_vPos; 

    void Start ()
    {
        /*int nCurrentKeyAmount = 0;
        switch (GameManager.Instance.m_compBMSPlayer.eCurrentKeyNum)
        {
            case E_KEY_NUM.E_KEY_5K:
                {
                    nCurrentKeyAmount = 5;
                }
                break;

            case E_KEY_NUM.E_KEY_7K:
                {
                    nCurrentKeyAmount = 7;
                }
                break;
        }

        for (int i = 0; i < nCurrentKeyAmount; i++)
        {
            UnityEngine.Object objLineEffect = Resources.Load("Game/p_line_effect");
            GameObject newObj = (GameObject)Instantiate(objLineEffect, Vector3.zero, ((GameObject)objLineEffect).transform.rotation);
            newObj.transform.parent = gameObject.transform;
            newObj.transform.localPosition = new Vector3(Global.NOTE_POSITIONX[i] + 1.25f, 0, 0);
        }

        m_vPos = transform.position;*/
    }

    void Update()
    {
        /*m_fNoteSpeed = GameManager.Instance.m_compBMSPlayer.fNoteSpeed;
        m_fScrollAmount = GameManager.Instance.m_compBMSPlayer.scrollAmount;

        m_vPos = new Vector3(m_vPos.x, m_vPos.y, m_vPos.z - m_fScrollAmount);

        if (m_vPos.z < Global.JUDGE_POSITIONZ)
        {
            m_vPos = new Vector3(m_vPos.x, m_vPos.y, 250.0f);
        }

        gameObject.transform.position = new Vector3(m_vPos.x, m_vPos.y, m_vPos.z * m_fNoteSpeed);*/
    }
}
