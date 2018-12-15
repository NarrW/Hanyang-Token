using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

public class BMSLoader
{
    public BMSPlayer m_compBMSPlayer;

    public string   m_sFilePath = null;

    public string   m_sFolderPath = null;
    public string   m_sFileName = null;
    public string   m_sFolderName = null;

    public int m_nTotalLineCount   = 0;
    public int m_nTotalWav         = 0;
    public int m_nTotalBmp         = 0;
    public int m_nTotalNote        = 0;
    public int m_nKeyAmount        = 0;
    public int m_nNodeAmount       = 0;
    public int m_nLength           = 0;
    public float m_fBPM            = 0.0f;

    bool        m_bOnceCheckConvertingBMP = false;  // 비트맵 변환 여부 체크
    bool        m_bSkipConvertingBMP = false;       // 비트맵 변환 과정 넘겨도 되는지

    bool        m_bIsStageFileExist = true;

    public class BMSInfo
    {
        public string sPlayer = "";
	    public string sBpm = "";
	    public string sPlaylevel = "";
	    public string sRank = "";
	    public string sGenre = "";
	    public string sTitle = "";
	    public string sArtist = "";
	    public string sVolwav = "";
	    public string sStagefile = "";
	    public string sTotal = "";
	    public string sMidifile = "";
	    public string sVideofile = "";
	    public string sSubartist = "";
	    public string sBanner = "";
	    public string sDifficulty = "";
	    public string sBackBmp = "";
    }

    public BMSInfo     m_cBMSInfo;

    public BMSSectionManager m_cSectionManager;

    public Dictionary<string, BMSMediaFile> m_dicBMP;
    public Dictionary<string, BMSMediaFile> m_dicWAV;
    public Dictionary<string, float> m_dicBPM;
    public Dictionary<string, float> m_dicSTOP;

    public E_LONGNOTE_TYPE eCurrentLongNoteType;
    public string sLNOBJValue;

    public void Init(BMSPlayer compBMSPlayer, string sPath)
    {
        m_compBMSPlayer = compBMSPlayer;

        m_sFilePath = sPath;

        m_sFolderPath = Path.GetDirectoryName(m_sFilePath);
        m_sFileName = Path.GetFileName(m_sFilePath);
        m_sFolderName = Path.GetFileName(m_sFolderPath);

        m_cBMSInfo = new BMSInfo();
    }

    public void LoadMetaData()
    {
        using (FileStream fs = File.Open(m_sFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (BufferedStream bs = new BufferedStream(fs))
        //TextAsset textFile = (TextAsset)Resources.Load(_full_name, typeof(TextAsset));
        //using (Stream bs = GenerateStreamFromString(textFile.text))
        using (StreamReader sr = new StreamReader(bs, Encoding.Default, true))
        {
            string linedata;
            do
            {
                linedata = sr.ReadLine();
                ParseMetaData(linedata);

                m_nTotalLineCount++;
            } while (linedata != null);

            sr.Close();
            fs.Close();
        }

        m_nLength = (int)((4.0f * (float)m_nNodeAmount) / (m_fBPM / 60.0f));
    }

    public void LoadMainData()
    {
        m_cSectionManager = new BMSSectionManager(m_compBMSPlayer);

        m_dicBMP = new Dictionary<string, BMSMediaFile>();
        m_dicWAV = new Dictionary<string, BMSMediaFile>();
        m_dicBPM = new Dictionary<string, float>();
        m_dicSTOP = new Dictionary<string, float>();

        eCurrentLongNoteType = E_LONGNOTE_TYPE.NONE;
        sLNOBJValue = "";

        using (FileStream fs = File.Open(m_sFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (BufferedStream bs = new BufferedStream(fs))
        //TextAsset textFile = (TextAsset)Resources.Load(_full_name, typeof(TextAsset));
        //using (Stream bs = GenerateStreamFromString(textFile.text))
        using (StreamReader sr = new StreamReader(bs, Encoding.Default, true))
        {
            string linedata;
            do
            {
                linedata = sr.ReadLine();
                ParseMainData(linedata);

                m_nTotalLineCount++;
            } while (linedata != null);

            sr.Close();
            fs.Close();
        }

        m_nLength = (int)((4.0f * (float)m_nNodeAmount) / (m_fBPM / 60.0f));
    }

    public void ParseMetaData(string linedata)
    {
        // 읽은 데이터가 없다면 리턴한다.
        if (linedata == null)
            return;

        if (linedata.StartsWith("#"))
        {
            char[] seps = new char[] { ' ' };
            string[] StringList = linedata.Split(seps, 2);

            if (StringList[0].Equals("#PLAYER", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sPlayer = StringList[1];
            }
            else if (StringList[0].Equals("#GENRE", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sGenre = StringList[1];
            }
            else if (StringList[0].Equals("#TITLE", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sTitle = StringList[1];
            }
            else if (StringList[0].Equals("#ARTIST", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sArtist = StringList[1];
            }
            else if (StringList[0].Equals("#BPM", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sBpm = StringList[1];
                m_fBPM = float.Parse(StringList[1]);
            }
            else if (StringList[0].Equals("#PLAYLEVEL", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sPlaylevel = StringList[1];
            }
            else if (StringList[0].Equals("#RANK", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sRank = StringList[1];
            }
            else if (StringList[0].Equals("#VOLWAV", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sVolwav = StringList[1];
            }
            else if (StringList[0].Equals("#STAGEFILE", StringComparison.InvariantCultureIgnoreCase))
            {
                {
                    if (StringList[1] == "")
                    {
                        m_cBMSInfo.sStagefile = "";
                        m_bIsStageFileExist = false;
                    }
                    else
                    {
                        string path = m_sFolderPath + "/" + StringList[1];

                        string woext = Path.ChangeExtension(path, null);
                        string ext = Path.GetExtension(path);

                        if (ext == ".bmp")
                        {
                            if (File.Exists(woext + ".png") != true
                                && File.Exists(woext + ".bmp") == true)
                            {
                                Bitmap bmp = new Bitmap(path);
                                bmp.Save(woext + ".png", ImageFormat.Png);
                            }

                            path = woext + ".png";
                        }

                        m_cBMSInfo.sStagefile = Path.GetFileName(path);
                    }
                }
            }
            else if (StringList[0].Equals("#TOTAL", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sTotal = StringList[1];
            }
            else if (StringList[0].Equals("#MIDIFILE", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sMidifile = StringList[1];
            }
            else if (StringList[0].Equals("#VIDEOFILE", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sVideofile = StringList[1];
            }
            else if (StringList[0].Equals("#SUBARTIST", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sSubartist = StringList[1];
            }
            else if (StringList[0].Equals("#BANNER", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sBanner = StringList[1];
            }
            else if (StringList[0].Equals("#DIFFICULTY", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sDifficulty = StringList[1];
            }
            else if (StringList[0].Equals("#BACKBMP", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sBackBmp = StringList[1];
            }
            else
            {
                string type = StringList[0].Substring(0, 4);

                #region LONGNOTE
                if (type.Equals("#LNO", StringComparison.InvariantCultureIgnoreCase))
                {
                    eCurrentLongNoteType = E_LONGNOTE_TYPE.LNOBJ;
                    sLNOBJValue = StringList[1];
                }
                else if (type.Equals("#LNT", StringComparison.InvariantCultureIgnoreCase))
                {
                    int nNumber = Int32.Parse(StringList[1]);
                    if (nNumber == 1)
                    {
                        eCurrentLongNoteType = E_LONGNOTE_TYPE.LNTYPE1;
                    }
                    else if (nNumber == 2)
                    {
                        eCurrentLongNoteType = E_LONGNOTE_TYPE.LNTYPE2;
                    }
                }
                #endregion

                #region WAVFILE
                else if (type.Equals("#WAV", StringComparison.InvariantCultureIgnoreCase))
                {

                }
                #endregion

                #region BMPFILE
                else if (type.Equals("#BMP", StringComparison.InvariantCultureIgnoreCase))
                {
                    /*if(m_bIsStageFileExist == false)
                    {
                        m_bIsStageFileExist = true;

                        string index = StringList[0].Substring(4, 2);
                        string path = m_sFolderPath + "/" + StringList[1];

                        string woext = Path.ChangeExtension(path, null);
                        string ext = Path.GetExtension(path);

                        if (ext == ".bmp")
                        {
                            if (File.Exists(woext + ".png") == false)
                            {
                                Bitmap bmp = new Bitmap(path);
                                bmp.Save(woext + ".png", ImageFormat.Png);
                            }

                            path = woext + ".png";
                            ext = ".png";
                        }

                        m_cBMSInfo.sStagefile = Path.GetFileName(path);
                    }*/
                }
                #endregion

                #region BPMCHANGE
                else if (type.Equals("#BPM", StringComparison.InvariantCultureIgnoreCase))
                {

                }
                #endregion

                #region STOPCHANGE
                else if (type.Equals("#STO", StringComparison.InvariantCultureIgnoreCase))
                {

                }
                #endregion

                #region NOTE
                else
                {
                    string check = StringList[0].Substring(6, 1);
                    if (check == ":")
                    {
                        char[] noteSeps = new char[] { ':' };
                        string[] NoteString = linedata.Split(noteSeps, 2);

                        //!< Bar 번호 (int 형으로 변형해 줘야함)
                        int nNodeNumber = Int32.Parse(NoteString[0].Substring(1, 3));

                        if (m_nNodeAmount != nNodeNumber)
                        {
                            m_nNodeAmount = nNodeNumber;
                        }

                        //!< 채널 번호 첫번째 0번이면 채널 뒷번호를 이용한 건반 채널로직으로 처리
                        //!< 1이면 채널 뒷번호를 이용한 건반 채널 생성
                        int nChannelFirstValue = Int32.Parse(NoteString[0].Substring(4, 1));

                        //!< 채널 번호 두번째 
                        //!< 1 : 배경음 (wave 채널) 2 : 마디 단축 3 : BPM 채널 4 : BGA 채널 5: BM95 확장 채널
                        //!< 6 : poor bga 채널 7 : 8 : 9 : 
                        int nChannelSecondValue = Int32.Parse(NoteString[0].Substring(5, 1));

                        //!< 뒷 데이터
                        string sData = NoteString[1];

                        int nKeyIndex = 0;
                        // 건반 채널
                        if (nChannelFirstValue == 1
                            || nChannelFirstValue == 2)
                        {
                            switch (nChannelSecondValue)
                            {
                                case (int)E_KEYBOARD_TYPE.E_KEYBOARD_SIX:
                                    nKeyIndex = 0;
                                    break;
                                case (int)E_KEYBOARD_TYPE.E_KEYBOARD_ONE:
                                    nKeyIndex = 1;
                                    break;
                                case (int)E_KEYBOARD_TYPE.E_KEYBOARD_TWO:
                                    nKeyIndex = 2;
                                    break;
                                case (int)E_KEYBOARD_TYPE.E_KEYBOARD_THREE:
                                    nKeyIndex = 3;
                                    break;
                                case (int)E_KEYBOARD_TYPE.E_KEYBOARD_FOUR:
                                    nKeyIndex = 4;
                                    break;
                                case (int)E_KEYBOARD_TYPE.E_KEYBOARD_FIVE:
                                    nKeyIndex = 5;
                                    break;
                                case (int)E_KEYBOARD_TYPE.E_KEYBOARD_EIGHT:
                                    nKeyIndex = 6;
                                    break;
                                case (int)E_KEYBOARD_TYPE.E_KEYBOARD_NINE:
                                    nKeyIndex = 7;
                                    break;
                            }

                            nKeyIndex = nKeyIndex * nChannelFirstValue;

                            if (nKeyIndex > m_nKeyAmount)
                            {
                                m_nKeyAmount = nKeyIndex;
                            }

                            int nLength = sData.Length;
                            if (nLength % 2 != 0) nLength -= 1;

                            nLength /= 2;

                            for (int i = 0; i < nLength; i++)
                            {
                                string nTemp = sData.Substring(i * 2, 2);

                                if (nTemp == "00") continue;

                                m_nTotalNote += 1;
                            }
                        }
                    }
                }
                #endregion
            }
            /*else
            {
                // #으로 시작하지 않는 문자는 모두 무시한다.
                return;
            }*/
        }
    }

    public void ParseMainData(string linedata)
    {
        if (linedata == null)
            return;

        if (linedata.StartsWith("#"))
        {
            char[] seps = new char[] { ' ' };
            string[] StringList = linedata.Split(seps, 2);

            if (StringList[0].Equals("#PLAYER", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sPlayer = StringList[1];
            }
            else if (StringList[0].Equals("#GENRE", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sGenre = StringList[1];
            }
            else if (StringList[0].Equals("#TITLE", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sTitle = StringList[1];
            }
            else if (StringList[0].Equals("#ARTIST", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sArtist = StringList[1];
            }
            else if (StringList[0].Equals("#BPM", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sBpm = StringList[1];
                m_fBPM = float.Parse(StringList[1]);
            }
            else if (StringList[0].Equals("#PLAYLEVEL", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sPlaylevel = StringList[1];
            }
            else if (StringList[0].Equals("#RANK", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sRank = StringList[1];
            }
            else if (StringList[0].Equals("#VOLWAV", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sVolwav = StringList[1];
            }
            else if (StringList[0].Equals("#STAGEFILE", StringComparison.InvariantCultureIgnoreCase))
            {
                {
                    string path = m_sFolderPath + "/" + StringList[1];

                    string woext = Path.ChangeExtension(path, null);
                    string ext = Path.GetExtension(path);

                    if (ext == ".bmp")
                    {
                        if (File.Exists(woext + ".png") != true
                            && File.Exists(woext + ".bmp") == true)
                        {
                            Bitmap bmp = new Bitmap(path);
                            bmp.Save(woext + ".png", ImageFormat.Png);
                        }

                        path = woext + ".png";
                    }

                    m_cBMSInfo.sStagefile = Path.GetFileName(path);
                }
            }
            else if (StringList[0].Equals("#TOTAL", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sTotal = StringList[1];
            }
            else if (StringList[0].Equals("#MIDIFILE", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sMidifile = StringList[1];
            }
            else if (StringList[0].Equals("#VIDEOFILE", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sVideofile = StringList[1];
            }
            else if (StringList[0].Equals("#SUBARTIST", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sSubartist = StringList[1];
            }
            else if (StringList[0].Equals("#BANNER", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sBanner = StringList[1];
            }
            else if (StringList[0].Equals("#DIFFICULTY", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sDifficulty = StringList[1];
            }
            else if (StringList[0].Equals("#BACKBMP", StringComparison.InvariantCultureIgnoreCase))
            {
                m_cBMSInfo.sBackBmp = StringList[1];
            }
            else
            {
                string type = StringList[0].Substring(0, 4);

                #region LONGNOTE
                if (type.Equals("#LNO", StringComparison.InvariantCultureIgnoreCase))
                {
                    eCurrentLongNoteType = E_LONGNOTE_TYPE.LNOBJ;
                    sLNOBJValue = StringList[1];
                }
                else if (type.Equals("#LNT", StringComparison.InvariantCultureIgnoreCase))
                {
                    int nNumber = Int32.Parse(StringList[1]);
                    if (nNumber == 1)
                    {
                        eCurrentLongNoteType = E_LONGNOTE_TYPE.LNTYPE1;
                    }
                    else if (nNumber == 2)
                    {
                        eCurrentLongNoteType = E_LONGNOTE_TYPE.LNTYPE2;
                    }
                }
                #endregion

                #region WAVFILE
                else if (type.Equals("#WAV", StringComparison.InvariantCultureIgnoreCase))
                {
                    //!< 구간 두개로 자릅니다.
                    //!< 이유는 #WAV0U uptec - Marker #59.wav 이러한 형식으로 되어있기 때문에 
                    //!< #WAVOU // uptec - Marker #59 //.wav 이렇게 잘라버립니다.
                    //!< 그리고 또 substr로 문자열을 잘라버립니다.
                    //!< #WAVOU 에서 OU가 해당 wav의 번호이기 때문입니다. 

                    string index = StringList[0].Substring(4, 2);
                    string path = m_sFolderPath + "/" + StringList[1];
                    string ext = Path.GetExtension(path);

                    if (File.Exists(path) == false)
                    {
                        if (ext == ".wav")
                        {
                            ext = ".ogg";
                            path = Path.ChangeExtension(path, ".ogg");
                        }
                        else if (ext == ".ogg")
                        {
                            ext = ".wav";
                            path = Path.ChangeExtension(path, ".wav");
                        }
                    }

                    m_dicWAV.Add(index, new BMSMediaFile(index, path, Path.GetFileNameWithoutExtension(path), ext));
                }
                #endregion

                #region BMPFILE
                else if (type.Equals("#BMP", StringComparison.InvariantCultureIgnoreCase))
                {
                    //!< 구간 두개로 자릅니다.
                    //!< 이유는 #WAV0U uptec - Marker #59.wav 이러한 형식으로 되어있기 때문에 
                    //!< #WAVOU // uptec - Marker #59 //.wav 이렇게 잘라버립니다.
                    //!< 그리고 또 substr로 문자열을 잘라버립니다.
                    //!< #WAVOU 에서 OU가 해당 wav의 번호이기 때문입니다. 

                    string index = StringList[0].Substring(4, 2);
                    string path = m_sFolderPath + "/" + StringList[1];

                    string woext = Path.ChangeExtension(path, null);
                    string ext = Path.GetExtension(path);

                    if (ext == ".bmp")
                    {
                        if (m_bOnceCheckConvertingBMP == false && m_bSkipConvertingBMP == false)
                        {
                            if (File.Exists(woext + ".png") != true)
                            {
                                m_bOnceCheckConvertingBMP = true;
                            }
                            else
                            {
                                m_bSkipConvertingBMP = true;
                            }
                        }

                        if (m_bOnceCheckConvertingBMP == true && m_bSkipConvertingBMP == false)
                        {
                            Bitmap bmp = new Bitmap(path);
                            bmp.Save(woext + ".png", ImageFormat.Png);
                        }

                        path = woext + ".png";
                        ext = ".png";
                    }

                    m_dicBMP.Add(index, new BMSMediaFile(index, path, woext, ext));
                }
                #endregion

                #region BPMCHANGE
                else if (type.Equals("#BPM", StringComparison.InvariantCultureIgnoreCase))
                {
                    string index = StringList[0].Substring(4, 2);
                    string value = StringList[1];

                    float fValue = float.Parse(value);
                    //if (fValue >= 10240)
                    //fValue = 10240;

                    m_dicBPM.Add(index, fValue);
                }
                #endregion

                #region STOPCHANGE
                else if (type.Equals("#STO", StringComparison.InvariantCultureIgnoreCase))
                {
                    string index = StringList[0].Substring(5, 2);
                    string value = StringList[1];

                    float fValue = float.Parse(value);

                    m_dicSTOP.Add(index, fValue);
                }
                #endregion

                #region NOTE
                else
                {
                    string check = StringList[0].Substring(6, 1);
                    if (check == ":")
                    {
                        char[] noteSeps = new char[] { ':' };
                        string[] NoteString = linedata.Split(noteSeps, 2);

                        //!< Bar 번호 (int 형으로 변형해 줘야함)
                        int nNodeNumber = Int32.Parse(NoteString[0].Substring(1, 3));

                        if (m_nNodeAmount != nNodeNumber)
                        {
                            m_nNodeAmount = nNodeNumber;
                        }

                        //!< 채널 번호 첫번째 0번이면 채널 뒷번호를 이용한 건반 채널로직으로 처리
                        //!< 1이면 채널 뒷번호를 이용한 건반 채널 생성
                        int nChannelFirstValue = Int32.Parse(NoteString[0].Substring(4, 1));

                        //!< 채널 번호 두번째 
                        //!< 1 : 배경음 (wave 채널) 2 : 마디 단축 3 : BPM 채널 4 : BGA 채널 5: BM95 확장 채널
                        //!< 6 : poor bga 채널 7 : 8 : 9 : 
                        int nChannelSecondValue = Int32.Parse(NoteString[0].Substring(5, 1));

                        //!< 뒷 데이터
                        string sData = NoteString[1];

                        //!< 뒷 데이터 마디 갯수
                        //int nData = sData.Length / 2;

                        m_cSectionManager.ProcessSection(nNodeNumber, nChannelFirstValue, nChannelSecondValue, sData);
                    }
                }
                #endregion
            }
        }
    }
}
