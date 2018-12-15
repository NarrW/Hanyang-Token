using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectedPanel : MonoBehaviour
{
    public Text uiTitle;
    public Text uiArtist;
    public Text uiGenre;
    public Text uiKey;
    public Text uiLevel;

    public Text uiTime;
    public Text uiBpm;
    public Text uiNote;

    public float m_fCurTime = 0.0f;
    public float m_fDestTime = 0.0f;
    public float m_fVelocityTime = 0.0f;
    public float m_fMin = 0.0f;
    public float m_fSec = 0.0f;
    public string m_sMin = "";
    public string m_sSec = "";

    public float m_fCurBpm = 0.0f;
    public float m_fDestBpm = 0.0f;
    public float m_fVelocityBpm = 0.0f;

    public float m_fCurNote = 0.0f;
    public float m_fDestNote = 0.0f;
    public float m_fVelocityNote = 0.0f;

    void Awake()
    {
        
    }

    void Start ()
    {
	
	}
	
	void Update ()
    {
        m_fCurBpm = Mathf.SmoothDamp(m_fCurBpm, m_fDestBpm, ref m_fVelocityBpm, Time.deltaTime * 10);
        m_fCurNote = Mathf.SmoothDamp(m_fCurNote, m_fDestNote, ref m_fVelocityNote, Time.deltaTime * 10);
        m_fCurTime = Mathf.SmoothDamp(m_fCurTime, m_fDestTime, ref m_fVelocityTime, Time.deltaTime * 10);

        m_fMin = m_fCurTime / 60.0f;
        m_fSec = m_fCurTime % 60.0f;
        m_sMin = m_fMin.ToString("0");
        if(m_fSec < 10.0f)
        {
            m_sSec = "0" + m_fSec.ToString("0");
        }
        else
        {
            m_sSec = m_fSec.ToString("0");
        }

        uiBpm.text = m_fCurBpm.ToString("0");
        uiNote.text = m_fCurNote.ToString("0");
        uiTime.text = m_sMin + ":" + m_sSec;
    }

    public void SetSelectedInform(BMSLoader cBMSLoader)
    {
        SetSelectedInformText(cBMSLoader.m_cBMSInfo.sTitle,
            cBMSLoader.m_cBMSInfo.sArtist,
            cBMSLoader.m_cBMSInfo.sGenre,
            cBMSLoader.m_nKeyAmount.ToString() + "Key",
            cBMSLoader.m_cBMSInfo.sPlaylevel,
            cBMSLoader.m_nLength,
            cBMSLoader.m_fBPM,
            cBMSLoader.m_nTotalNote);
    }

    private void SetSelectedInformText(string sTitle, string sArtist, string sGenre, string sKey, string sLevel, int nTime, float fBpm, int nNote)
    {
        uiTitle.text = sTitle;
        uiArtist.text = sArtist;
        uiGenre.text = sGenre;
        uiKey.text = sKey;
        uiLevel.text = sLevel;

        m_fDestTime = nTime;
        m_fDestBpm = fBpm;
        m_fDestNote = nNote;

        //Debug.Log(m_fDestBpm.ToString());
        //Debug.Log(m_fDestNote.ToString());
    }
}
