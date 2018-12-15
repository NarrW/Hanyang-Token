using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BMSSection
{
    public BMSPlayer m_compBMSPlayer;

    public int m_nSectionNumber = 0;

    public float m_fSectionHeight = 0.0f;
    public float m_fSectionScale = 1.0f;

    public float m_fSectionStartPos = 0.0f;

    public List<BMSKey> m_listKey;

    public BMSSectionManager m_cCurrentBMSSectionList;

    public BMSSection(BMSPlayer compBMSPlayer)
    {
        m_compBMSPlayer = compBMSPlayer;

        m_listKey = new List<BMSKey>();
    }

    public void AddKey(int nChannelFirst, int nChannelSecond, string data)
    {
        BMSKey cKey = new BMSKey(nChannelFirst, nChannelSecond, data);
        if (nChannelFirst == 0 && nChannelSecond == 2)
            m_listKey.Insert(0, cKey);
        else
            m_listKey.Add(cKey);
    }

    public void CreateAllNotes()
    {
        foreach (BMSKey cKey in m_listKey)
        {
            CreateNote(cKey.m_nChannelFirst, cKey.m_nChannelSecond, cKey.m_sData);
        }
    }

    public void CreateNote(int nChannelFirst, int nChannelSecond, string data)
    {
        E_CHANNEL_TYPE eChannelType = (E_CHANNEL_TYPE)nChannelFirst;

        //!< Barnum을 이용하여 y 블럭의 위치 설정
        switch (eChannelType)
        {
            case E_CHANNEL_TYPE.E_CHANNEL_TYPE_EVENT:
                //Debug.Log("Add Event " + nChannelFirst.ToString() + " " + nChannelSecond.ToString() + " " + data);
                AddEventBlock(nChannelSecond, data);
                break;

            case E_CHANNEL_TYPE.E_CHANNEL_TYPE_P1:
            case E_CHANNEL_TYPE.E_CHANNEL_TYPE_LN1:
                //Debug.Log("Add Note " + nChannelFirst.ToString() + " " + nChannelSecond.ToString() + " " + data);
                AddKeyBlock(nChannelFirst, nChannelSecond, data);
                break;
        }
    }

    public void AddNodeLine()
    {
        float y = m_fSectionStartPos + Global.JUDGE_POSITIONZ;
        m_compBMSPlayer.AddNodeLine(y, this);
    }

    public void AddEventBlock(int nChannelSecond, string sData)
    {
        //BlockProcess(0, 0, data, (E_NOTE_CHANNEL)nChannelNum);

        E_EVENT_CHANNEL nEventChannel = (E_EVENT_CHANNEL)nChannelSecond;

        int nLength = sData.Length;
        if (nLength % 2 != 0) nLength -= 1;

        nLength /= 2;

        for (int i = 0; i < nLength; i++)
        {
            string nTemp = sData.Substring(i * 2, 2);

            if (nTemp == "00") continue;

            float y = ((float)i / (float)nLength) * m_fSectionHeight + m_fSectionStartPos + Global.JUDGE_POSITIONZ;

            m_compBMSPlayer.AddEventBlock(nEventChannel, sData.Substring(i * 2, 2), y, this);
        }
    }

    public void AddKeyBlock(int nPlayer, int nKeyIndex, string sData)
    {
        int nLength = sData.Length;
        if (nLength % 2 != 0) nLength -= 1;

        nLength /= 2;

        for (int i = 0; i < nLength; i++)
        {
            string nTemp = sData.Substring(i * 2, 2);

            if (nTemp == "00") continue;

            float y = ((float)i / (float)nLength) * m_fSectionHeight + m_fSectionStartPos + Global.JUDGE_POSITIONZ;

            m_compBMSPlayer.AddKeyBlock(nPlayer, nKeyIndex, sData.Substring(i * 2, 2), y, this);
        }
    }
}
