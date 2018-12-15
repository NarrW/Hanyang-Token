using UnityEngine;
using System;
using System.Collections;

public class BMSShortNote : BMSNote
{
    public BMSPlayer m_compBMSPlayer;

    public void Init(BMSPlayer compBMSPlayer, int nNoteKey, float fPosZ)
    {
        m_compBMSPlayer = compBMSPlayer;

        m_eEventChannel = E_EVENT_CHANNEL.E_CHANNEL_NONE;
        m_eNoteType = E_NOTE_TYPE.NORMAL;

        m_objLongBody = null;
        m_compRendererLongBody = null;
        m_fLongBodyInitialScale = 1.0f;

        m_bIsShow = false;
        m_bActive = true;
        m_bIsKilled = false;
        m_bSong = false;
        m_bIsPlayArea = false;
        m_bIsPassedJudgeLine = false;

        m_sIndex = null;
        m_nKeyIndex = 0;
        m_nNoteKey = nNoteKey;
        m_nSoundNum = 0;

        m_fBpm = 0.0f;
        m_fStop = 0.0f;
        m_fCanselTime = 0.0f;
        m_fDestTime = 0.0f;

        m_dBarperSecond = 0.0f;
        m_dBarDownTime = 0.0f;

        m_texBGA = null;

        m_vPos = new Vector3(m_compBMSPlayer.NOTE_POSITIONX[m_nNoteKey], 0.0f, (float)fPosZ);
        transform.position = new Vector3(m_compBMSPlayer.NOTE_POSITIONX[m_nNoteKey], 0.0f, (float)fPosZ);

        if (m_bIsSeperator == false)
        {
            m_compRenderer = GetComponent<SpriteRenderer>();
            m_compRenderer.enabled = false;
        }
    }

    public override void Judge()
    {
        
    }

    public override void SemiKill()
    {
        if (m_bIsKilled == true)
            return;

        m_bIsKilled = true;

        m_dBarDownTime = 0.0f;

        if (m_objLongBody != null)
        {
            //m_objLongBody.SetActive(false);
            m_compRendererLongBody.color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
        }

        //m_bActive = false;

        m_compRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
        //gameObject.SetActive(false);

        m_compBMSPlayer.SetNextCurrentNote(m_nNoteKey);
    }

    public override void Kill()
    {
        if (m_bIsKilled == true)
            return;

        m_bIsKilled = true;

        m_dBarDownTime = 0.0f;

        if (m_objLongBody != null)
        {
            m_objLongBody.SetActive(false);
        }

        m_bActive = false;

        gameObject.SetActive(false);        

        switch(m_eChannelType)
        {
            case E_CHANNEL_TYPE.E_CHANNEL_TYPE_EVENT:
                {
                    // 이벤트 노트

                    switch (m_eEventChannel)
                    {
                        case E_EVENT_CHANNEL.E_CHANNEL_BGM:
                            {
                                PlaySound();
                            }
                            break;

                        case E_EVENT_CHANNEL.E_CHANNEL_BPM:
                            {
                                m_compBMSPlayer.nBPM = m_fBpm;
                            }
                            break;

                        case E_EVENT_CHANNEL.E_CHANNEL_BGA:
                            {
                                if (m_bIsVideo == true)
                                {
                                    m_compBMSPlayer.PlayVideo(m_sIndex);
                                }
                                else
                                {
                                    if (m_texBGA == null)
                                    {
                                        m_compBMSPlayer.m_compBackgroundImage.texture = null;
                                    }
                                    else
                                    {
                                        m_compBMSPlayer.m_compBackgroundImage.texture = m_texBGA;
                                    }
                                }
                            }
                            break;

                        case E_EVENT_CHANNEL.E_CHANNEL_EX_BPM:
                            {
                                m_compBMSPlayer.nBPM = m_fBpm;
                            }
                            break;

                        case E_EVENT_CHANNEL.E_CHANNEL_STOP:
                            {
                                //GameManager.Instance.m_compBMSPlayer.nJointValue = 0;
                                //GameManager.Instance.m_compBMSPlayer.fStopTime = m_fStop * Global.MAX_NOTEFIELD_HEIGHT;

                                m_compBMSPlayer.fStopFactor = m_fStop;
                                m_compBMSPlayer.bIsWait = true;
                            }
                            break;
                    }
                }
                break;

            default:
                {
                    // 키 노트
                    m_compBMSPlayer.SetNextCurrentNote(m_nNoteKey);

                    switch (m_eNoteType)
                    {
                        case E_NOTE_TYPE.NORMAL:
                            {
                                //Debug.Log("NORMAL");
                                //PlaySound();
                            }
                            break;

                        case E_NOTE_TYPE.LONG_START:
                            {
                                //PlaySound();
                            }
                            break;

                        case E_NOTE_TYPE.LONG_END:
                            {
                                //Debug.Log("LONG_END");
                            }
                            break;
                    }
                }
                break;
        }
    }

    public override void PlaySound()
    {
        BassManager.Instance.PlayStream(m_nSoundNum, true);
    }

	public override void Start ()
    {
        
	}

    /*public override void Update()
    {
        if (m_bActive == false)
        {
            return;
        }

        MoveTime(Time.deltaTime);

        //!< 노트 타입이 10보다 작으면 (9까지 키보드 노트 들)
        if (m_eChannel == E_NOTE_CHANNEL.E_CHANNEL_KEY)
        {
            if (m_bIsPlayArea == false)
            {
                if (transform.position.y < 100.0f)
                {
                    m_bIsPlayArea = true;
                    m_compMeshRenderer.enabled = true;
                }
            }

            //if (m_vPos.y >= Global.JUDGE_POSITIONZ - Global.fJudgePosPoor)
            if (m_vPos.y > Global.JUDGE_POSITIONZ)
            {
                m_vPos = new Vector3(m_vPos.x, m_vPos.y - m_dBarDownTime, m_vPos.z);
                m_dBarDownTime = 0.0f;
                transform.position = new Vector3(m_vPos.x, m_vPos.y * GameManager.Instance.m_compBMSPlayer.fNoteSpeed, m_vPos.z);
                //m_pNote->getAnimation()->setPosition(ccp(m_pNote->getPos().x, (dTemp - D_JUDGE_POSITIONZ) * D_ASSET_MNG->getAttachment() + D_JUDGE_POSITIONZ));
            }
            else
            {
                //m_bMiss = true;
                //D_ASSET_MNG->setCombo(0);

                Kill();
            }
        }
        else
        {
            if (m_vPos.y > Global.JUDGE_POSITIONZ)
            {
                m_vPos = new Vector3(m_vPos.x, m_vPos.y - m_dBarDownTime, m_vPos.z);
                m_dBarDownTime = 0.0f;
                transform.position = new Vector3(m_vPos.x, m_vPos.y * GameManager.Instance.m_compBMSPlayer.fNoteSpeed, m_vPos.z);
            }
            else
            {
                //m_fDestTime = 2.0f;

                Kill();
            }
        }
    }*/

    /*public void MoveTime(float dt)
    {
        //m_dBarperSecond = ((4 * 60) / GameManager.Instance.m_compBMSPlayer.nBPM);
        m_dBarDownTime = GameManager.Instance.m_compBMSPlayer.nBPM / 4.0f / 60.0f * Global.MAX_NOTEFIELD_HEIGHT * dt;
        //m_fDestTime = (((m_vPos.y - Global.JUDGE_POSITIONZ) / (Global.MAX_NOTEFIELD_HEIGHT * dt / ((4 * 60) / GameManager.Instance.m_compBMSPlayer.nBPM))) * dt) + 0.2f;

        if (GameManager.Instance.m_compBMSPlayer.nJointValue == 0)
        {
            m_dBarDownTime = 0.0f;
        }
    }*/
}
