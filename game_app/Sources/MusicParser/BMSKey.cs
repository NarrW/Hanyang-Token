using UnityEngine;
using System.Collections;

public class BMSKey
{
    public int      m_nChannelFirst;
    public int      m_nChannelSecond;
    public string   m_sData;

    public BMSKey(int nChannelFirst, int nChannelSecond, string sData)
    {
        m_nChannelFirst = nChannelFirst;
        m_nChannelSecond = nChannelSecond;
        m_sData = sData;
    }
}
