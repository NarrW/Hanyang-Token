using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BMSSectionManager
{
    public BMSPlayer m_compBMSPlayer;

    public Dictionary<int, BMSSection> m_dicSection;
    public int m_nSectionCursor;

    public float m_fSectionHeightTotal;

    public BMSSectionManager(BMSPlayer compBMSPlayer)
    {
        m_compBMSPlayer = compBMSPlayer;

        m_dicSection = new Dictionary<int, BMSSection>();

        m_nSectionCursor = 0;

        m_fSectionHeightTotal = 0.0f;

        AddSection(0);
    }

    public void AddSection(int nSectionNumber)
    {
        if (m_dicSection.ContainsKey(nSectionNumber) == true)
            return;

        BMSSection cSection = new BMSSection(m_compBMSPlayer);
        cSection.m_fSectionHeight = Global.MAX_NOTEFIELD_HEIGHT;
        cSection.m_cCurrentBMSSectionList = this;
        cSection.m_nSectionNumber = nSectionNumber;
        m_dicSection.Add(nSectionNumber, cSection);
    }

    public void AddKeyToSection(int nSectionNumber, int nChannelFirst, int nChannelSecond, string sData)
    {
        m_dicSection[nSectionNumber].AddKey(nChannelFirst, nChannelSecond, sData);
        //Debug.Log("Add Key " + nSectionNumber.ToString() + " " + sData);
    }

    public void SetSectionScale(int nSectionNumber, float fSectionScale)
    {
        m_dicSection[nSectionNumber].m_fSectionScale = fSectionScale;
        m_dicSection[nSectionNumber].m_fSectionHeight = Global.MAX_NOTEFIELD_HEIGHT * fSectionScale;
    }

    public void CreateAllNotes()
    {
        for (int i = 0; i < m_dicSection.Count; i++)
        {
            BMSSection cSection = m_dicSection[i];

            cSection.m_fSectionStartPos = m_fSectionHeightTotal;

            cSection.AddNodeLine();

            m_fSectionHeightTotal += cSection.m_fSectionHeight;

            cSection.CreateAllNotes();

            m_compBMSPlayer.iLoadingPercentage += 1;
        }
    }
    public void CreateNotes(int nSectionNumber)
    {
        m_dicSection[nSectionNumber].CreateAllNotes();
    }

    public void ProcessSection(int nSectionNumber, int nChannelFirst, int nChannelSecond, string sData)
    {
        if (nSectionNumber > m_nSectionCursor)
        {
            // 빈 섹션 체크
            for (int i = m_nSectionCursor; i < nSectionNumber; i++)
            {
                AddSection(i);
            }

            // 새로 추가하는 섹션번호
            AddSection(nSectionNumber);
            AddKeyToSection(nSectionNumber, nChannelFirst, nChannelSecond, sData);

            m_nSectionCursor = nSectionNumber;
        }
        else
        {
            // 이미 있는 섹션번호
            AddKeyToSection(nSectionNumber, nChannelFirst, nChannelSecond, sData);
        }

        if (nChannelFirst == 0)
        {
            if (nChannelSecond == (int)E_EVENT_CHANNEL.E_CHANNEL_NODESCALE)
            {
                SetSectionScale(nSectionNumber, float.Parse(sData));
            }
        }
    }
}
