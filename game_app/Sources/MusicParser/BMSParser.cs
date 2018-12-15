using UnityEngine;

using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

public class BMSParser
{
    public StringBuilder fileContent;

    public string FullPath;
    public string FolderPath;
    public string FileName;
    public string FolderName;

    public byte PlayerNumber;
    public string Genre;
    public string Title;
    public string Artist;
    public double BPM;
    public byte Level;
    public byte Rank;
    public int Volume;
    public double Total;
    public string StageFile;

    public byte Line;
    public int NoteCount = 0;
    public double SongLength = 0.0;
    
    public bool IsFinishLoadMainData;

    public int DivideMax;
    public int BMPCount;

    public BMSParser()
    {
        fileContent = new StringBuilder("");

        IsFinishLoadMainData = false;

        wavDic = new Dictionary<string, string>();
        bmpDic = new Dictionary<string, string>();
        BPMDic = new Dictionary<string, double>();
        stopDic = new Dictionary<string, double>();

        BMPCount = 0;
    }

    #region wav / bmp / BPM / stop Dictionary
    public Dictionary<string, string> wavDic;
    public Dictionary<string, string> bmpDic;
    public Dictionary<string, double> BPMDic;
    public Dictionary<string, double> stopDic;
    #endregion

    #region NoteTypeEnum
    public enum NoteType
    {
        NORMAL,
        LONG_START,
        LONG_END
    }

    public enum LongNoteType
    {
        NONE,
        LNTYPE1,
        LNTYPE2,
        LNOBJ
    }

    public LongNoteType CurrentLongNoteType;
    string LNOBJValue;
    #endregion

    #region Node Class
    public class Node
    {
        public int length;
        public double scale;

        public List<Note> noteList;
        public List<Event> eventList;
		public List<Event> bpmList;

        public double nodeLinePos;
        public double fixedNodeLength;

        public Node()
        {
            length = 0;
            scale = 1.0;

            noteList = new List<Note>();
            eventList = new List<Event>();
			bpmList = new List<Event>();

            nodeLinePos = 0;
            fixedNodeLength = 0.0;
        }
    }

    public Dictionary<int, Node> nodeList;
    #endregion

    #region Note Class
    public class Note
    {
        public GameObject NoteObject;
        public GameObject NoteLongBodyObject;

        public NoteType type;

        public int id;
        public int line;
        public int sequence;
        
        public int divSum;
        public double calcedSequence;
        
        public string value;
        public bool auto;

        public double pos;

        public bool isDead;
        
        public bool oncePlayLNOBJ;
        public double longDelay;

        public Note()
        {
            NoteObject = null;
            NoteLongBodyObject = null;

            type = NoteType.NORMAL;

            id = 0;
            line = 0;
            sequence = 0;

            divSum = 0;
            calcedSequence = 0.0;

            value = "";
            auto = false;

            pos = 0;

            isDead = false;

            oncePlayLNOBJ = false;
            longDelay = 0.0;
        }
    }
    #endregion

    #region Event Class
    public class Event
    {
        /* [Type]
        channel #01: 배경음 채널
            지정한 키값을 배경음으로 재생한다. 이 배경음은 어떤 경우에라도 항상 재생된다. 존재하지 않는 키값은 무시한다.
        channel #02: 마디 단축
            이 채널은 다른 채널과는 다르게 데이터로 숫자를 받는다. 지정한 실수 배로 해당 마디의 길이를 줄이거나 늘린다. (0.5일 경우 원래 길이의 반으로, 2.0일 경우 원래 길이의 두 배로) 이는 BPM의 조정과 다르며, 해당 마디가 재생되는 시간 자체가 그만큼 짧아지거나 길어지는 것을 의미한다.
        channel #03: BPM 채널
            해당하는 위치에서, BPM을 지정한 16진수 숫자(1부터 255까지)로 바꾼다. 예를 들어서 78은 BPM 120을 의미한다.
        channel #04: BGA 채널
            현재 표시되고 있는 배경 이미지 파일을 해당하는 키값에 지정된 이미지 파일로 바꾼다. 지정된 이미지는 다른 이미지가 이 채널에서 지정될 때까지 계속 보여진다.
        channel #05: BM98 확장 채널
            이 채널은 BM98에서만 사용할 수 있다. 따라 오는 데이터들을 순서대로 해당 마디의 큐에 넣는다. 그리고 오브젝트 채널에서 명령을 내릴 때, 해당 마디의 큐가 비어 있지 않으면 나오는 순서대로 00이 아닌 키값에 큐에서 키값을 꺼내서 그 키값에 대응하는 스프라이트를 기본 오브젝트 모양 대신 출력한다. 키값에 대응하는 스프라이트 번호는 미리 지정되어 있으며 부록에 그 목록이 있다.
        channel #06: Poor BGA 채널
            사용자가 오브젝트를 입력하지 못 했을 때 표시될 배경 이미지를 해당 키값의 이미지로 바꾼다. 일반적인 BGA 채널과는 달리 Poor BGA 채널은 하나 뿐이며, 투명색 등은 모두 무시된다.
        channel #07: BGA 레이어 채널
            현재 표시되고 있는 배경 이미지 파일을 해당하는 키값에 지정된 이미지 파일로 바꾼다. channel #04와 다른 점은 이 이미지는 channel #04에서 지정한 이미지 위에 표시된다는 것이다. (검은색, 즉 투명색으로 되어 있는 부분은 아래의 일반 BGA 이미지가 비쳐 보이게 된다.) 그래픽 편집 툴의 레이어 기능과 흡사하다.
        channel #08: 확장 BPM 채널
            해당하는 위치에서, BPM을 지정한 키값에 지정된 BPM 값(#BPMxx 명령으로 지정한 값)으로 변경한다.
        channel #09: 시퀀스 정지 채널
            해당하는 위치에서, 지정한 키값에 지정된 시간(#STOPxx 명령으로 지정한 값) 만큼 오브젝트가 움직이는 것을 멈춘다. 이미 재생되던 배경음은 그대로 재생되지만 다른 모든 것은 해당하는 시간만큼 멈춰 있게 된다. */

        public int type;
        public int sequence;

        public int divSum;
        public double calcedSequence;

        public string value;

        public double pos;

        public bool isDead;

        public Event()
        {
            type = 0;
            sequence = 0;

            divSum = 0;
            calcedSequence = 0.0;

            value = "";

            pos = 0;

            isDead = false;
        }
    }
    #endregion
	
    #region LoadInfo
    public void LoadInfo(string fullPath)
    {
        #if UNITY_ANDROID
			FullPath = "/mnt/sdcard/" + fullPath;
        #else
            FullPath = fullPath;
        #endif

        FolderPath = Path.GetDirectoryName(FullPath);

        FileName = Path.GetFileName(FullPath);
        FolderName = Path.GetFileName(FolderPath);

        byte lineCalc = 0;

        using (FileStream fs = File.Open(FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (BufferedStream bs = new BufferedStream(fs))
        //TextAsset textFile = (TextAsset)Resources.Load(_full_name, typeof(TextAsset));
        //using (Stream bs = GenerateStreamFromString(textFile.text))
        using (StreamReader sr = new StreamReader(bs/*, Encoding.GetEncoding("ks_c_5601-1987"), true*/))
        {
            string str;
            while ((str = sr.ReadLine()) != null)
            {
                if (str.Length > 6 && str.ElementAt(6) == ':')
                {
                    string mainDataStr = str.Substring(1);

                    // dividing the main data with word ":"
                    string firstData = mainDataStr.Substring(0, 5);
                    string secondData = mainDataStr.Substring(6);

                    int nodeNumber = Int32.Parse(firstData.Substring(0, 3));
                    int playerNumber = Int32.Parse(firstData.Substring(3, 1));

                    byte lineNumber = Convert.ToByte(firstData.Substring(4, 1));
                    lineNumber++;
                    if (lineNumber == 7)
                    { lineNumber = 1; }
                    else if (lineNumber == 8)
                    { lineNumber = 0; }
                    else if (lineNumber == 9)
                    { lineNumber = 7; }
                    else if (lineNumber == 10)
                    { lineNumber = 8; }

                    if (playerNumber != 0)
                    {
                        if (lineNumber > lineCalc)
                        {
                            lineCalc = lineNumber;
                        }
                    }
                }
                else
                {
                    //----- info field start -----//
                    if (str.Contains("#PLAYER "))
                    {
                        PlayerNumber = Convert.ToByte(str.Substring(7));
                    }
                    else if (str.Contains("#GENRE "))
                    {
                        Genre = str.Substring(7);
                    }
                    else if (str.Contains("#TITLE "))
                    {
                        Title = str.Substring(7);
                    }
                    else if (str.Contains("#ARTIST "))
                    {
                        Artist = str.Substring(8);
                    }
                    else if (str.Contains("#BPM ") && str.ElementAt(4).Equals(' '))
                    {
                        BPM = double.Parse(str.Substring(5));
                    }
                    else if (str.Contains("#PLAYLEVEL "))
                    {
                        Level = Convert.ToByte(str.Substring(11));
                    }
                    else if (str.Contains("#RANK "))
                    {
                        Rank = Convert.ToByte(str.Substring(6));
                    }
                    else if (str.Contains("#VOLWAV "))
                    {
                        Volume = Convert.ToInt16(str.Substring(8));
                    }
                    else if (str.Contains("#TOTAL "))
                    {
                        Total = Convert.ToSingle(str.Substring(7));
                    }
                    else if (str.Contains("#STAGEFILE "))
                    {
                        StageFile = str.Substring(11);
                    }
                }
            }

            sr.Close();
            fs.Close();
        }

        Line -= 1;

        if (PlayerNumber == 3)
        {
            Line = (byte)((int)lineCalc * 2);
        }
        else
        {
            Line = lineCalc;
        }
    }
    #endregion

    #region LoadMainData
    public void LoadMainData()
    {
        if (IsFinishLoadMainData == true)
        {
            return;
        }

        int CurrentNodeNumber;

        CurrentLongNoteType = LongNoteType.NONE;
        LNOBJValue = "";

        nodeList = new Dictionary<int, Node>();

        int id_count = 0;
        int data_max_length = 0;

        CurrentNodeNumber = 0;

        bool[] toggle_lntype_start_dic = new bool[Line + 1];
        for (int k = 1; k < Line + 1; k++)
        {
            toggle_lntype_start_dic[k] = true;
        }

        using (FileStream fs = File.Open(FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (BufferedStream bs = new BufferedStream(fs))
        //TextAsset textFile = (TextAsset)Resources.Load(full_name, typeof(TextAsset));
        //using (Stream bs = GenerateStreamFromString(textFile.text))
        using (StreamReader sr = new StreamReader(bs/*, Encoding.GetEncoding("ks_c_5601-1987"), true*/))
        {
            string str;
            while ((str = sr.ReadLine()) != null)
            {
                fileContent.Append(str);

                if (str.Length > 6 && str.ElementAt(6) == ':')
                {
                    string mainDataStr = "";
                    mainDataStr = str.Replace("#", "");

                    // dividing the main data with word ":"
                    string[] splitedMainDataStr = mainDataStr.Split(':'); ;

                    string firstData = splitedMainDataStr[0];
                    string secondData = splitedMainDataStr[1];

                    int second_data_length = secondData.Length;
                    if (data_max_length < second_data_length)
                    {
                        data_max_length = second_data_length;
                    }

                    int nodeNumber = Int32.Parse(firstData.Substring(0, 3));
                    int playerNumber = Int32.Parse(firstData.Substring(3, 1));

                    CurrentNodeNumber = nodeNumber;
                    if (nodeList.ContainsKey(CurrentNodeNumber) == false)
                    {
                        // add blank node in front of current node (if exist)
                        for (int i = 0; i < CurrentNodeNumber; i++)
                        {
                            if (nodeList.ContainsKey(i) == false)
                            {
                                nodeList.Add(i, new Node());
                            }
                        }

                        // add current node
                        nodeList.Add(CurrentNodeNumber, new Node());
                    }

                    ///////////////////////////////////
                    // fix key number to efficiently //
                    // SC 1 2 3 4 5 6 7              //
                    ///////////////////////////////////
                    int lineNumber = Int32.Parse(firstData.Substring(4, 1));
                    lineNumber++;
                    if (lineNumber == 7)
                    { lineNumber = 1; }
                    else if (lineNumber == 8)
                    { lineNumber = -99; }
                    else if (lineNumber == 9)
                    { lineNumber = 7; }
                    else if (lineNumber == 10)
                    { lineNumber = 8; }

                    #region Parsing (Note)
                    if (playerNumber != 0)
                    {
                        // '00' expection
                        if (!secondData.Equals("00"))
                        {
                            // division by 2
                            if (secondData.Length % 2 == 0)
                            {
                                double dataLength = secondData.Length;
                                double division_amount = dataLength / 2;

                                if (DivideMax < (int)division_amount)
                                {
                                    DivideMax = (int)division_amount;
                                }

                                for (int i = 0; i < dataLength; i += 2)
                                {
                                    string dividedData = secondData.Substring(i, 2);
                                    if (!dividedData.Equals("00"))
                                    {
                                        Note note = new Note();
                                        note.line = lineNumber;
                                        if (playerNumber == 2)
                                        {
                                            note.line = Line - (note.line - 1);
                                        }

                                        note.divSum = (int)division_amount;
                                        if (i == 0)
                                        {
                                            note.sequence = 0;
                                        }
                                        else
                                        {
                                            note.sequence = (i / 2);
                                        }
                                        note.calcedSequence = (double)note.sequence / (double)note.divSum;

                                        // set key wav
                                        if (wavDic.ContainsKey(dividedData))
                                        {
                                            note.value = dividedData;
                                        }
                                        else
                                        {
                                            note.value = "";
                                        }

                                        if (playerNumber == 1 || playerNumber == 2)
                                        {
                                            if (CurrentLongNoteType == LongNoteType.LNOBJ)
                                            {
                                                if (dividedData.Equals(LNOBJValue))
                                                {
                                                    note.type = NoteType.LONG_END;
                                                }
                                            }
                                        }
                                        else if (playerNumber == 5 || playerNumber == 6)
                                        {
                                            if (CurrentLongNoteType != LongNoteType.LNTYPE1)
                                                CurrentLongNoteType = LongNoteType.LNTYPE1;

                                            if (CurrentLongNoteType == LongNoteType.LNTYPE1)
                                            {
                                                if (toggle_lntype_start_dic[lineNumber] == true)
                                                {
                                                    //t_note.type = NoteType.LONG_START;
														
													toggle_lntype_start_dic[lineNumber] = false;
                                                }
												else if (toggle_lntype_start_dic[lineNumber] == false)
                                                {
                                                    note.type = NoteType.LONG_END;
														
													note.sequence = i / 2;
													note.calcedSequence = (double)note.sequence / (double)note.divSum;
														
													note.value = "";
														
													toggle_lntype_start_dic[lineNumber] = true;
                                                }
                                            }
                                        }

                                        // set unique id to note
                                        note.id = id_count;
                                        id_count++;

                                        nodeList[CurrentNodeNumber].noteList.Add(note);

                                        // counting note
                                        NoteCount++;
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region Parsing (Event)
                    else
                    {
                        int type = Int32.Parse(firstData.Substring(4, 1));
                        if (type == 2)
                        {
                            double t_node_scale = double.Parse(secondData);
                            nodeList[CurrentNodeNumber].scale = t_node_scale;
                        }
                        else
                        {
                            // '00' expection
                            if (!secondData.Equals("00"))
                            {
                                // division by 2
                                if (secondData.Length % 2 == 0)
                                {
                                    double dataLength = secondData.Length;
                                    double division_amount = dataLength / 2;

                                    if (DivideMax < (int)division_amount)
                                    {
                                        DivideMax = (int)division_amount;
                                    }

                                    for (int i = 0; i < dataLength; i += 2)
                                    {
                                        string dividedData = secondData.Substring(i, 2);
                                        if (!dividedData.Equals("00"))
                                        {
                                            Event t_event = new Event();
                                            t_event.type = type;

                                            t_event.divSum = (int)division_amount;
                                            if (i == 0)
                                            {
                                                t_event.sequence = 0;
                                            }
                                            else
                                            {
                                                t_event.sequence = (i / 2);
                                            }
                                            t_event.calcedSequence = (double)t_event.sequence / (double)t_event.divSum;

                                            if (type == 1)
                                            {
                                                // set key wav
                                                if (wavDic.ContainsKey(dividedData))
                                                {
                                                    t_event.value = dividedData;
                                                }
                                                else
                                                {
                                                    t_event.value = "";
                                                }

												nodeList[CurrentNodeNumber].eventList.Add(t_event);
                                            }
                                            else if (type == 3)
                                            {
												// bpm

                                                t_event.value = dividedData;

												nodeList[CurrentNodeNumber].bpmList.Add(t_event);
                                            }
                                            else if (type == 4 || type == 7)
                                            {
                                                // set bga image
                                                if (bmpDic.ContainsKey(dividedData))
                                                {
                                                    t_event.value = dividedData;
                                                }
                                                else
                                                {
                                                    t_event.value = "";
                                                }

												nodeList[CurrentNodeNumber].eventList.Add(t_event);
                                            }
                                            else if (type == 8)
                                            {
                                                // bpm
                                                if (BPMDic.ContainsKey(dividedData))
                                                {
                                                    t_event.value = dividedData;
                                                }
                                                else
                                                {
                                                    t_event.value = "";
                                                }

												nodeList[CurrentNodeNumber].bpmList.Add(t_event);
                                            }
                                            else if (type == 9)
                                            {
                                                // set stop
                                                if (stopDic.ContainsKey(dividedData))
                                                {
                                                    t_event.value = dividedData;
                                                    DivideMax = 192;
                                                }
                                                else
                                                {
                                                    t_event.value = "";
                                                }

												nodeList[CurrentNodeNumber].eventList.Add(t_event);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    //----- wav field start -----//
                    if (str.Contains("#WAV"))
                    {
                        LoadWAV(str);
                    }
                    //----- wav field end -----//


                    //----- bmp field start -----//
                    else if (str.Contains("#BMP"))
                    {
						#if UNITY_ANDROID
						#else
							LoadBMP(str);
						#endif
                    }
                    //----- bmp field end -----//


                    //----- bpm field start -----//
                    else if (str.Contains("#BPM") && !str.ElementAt(4).Equals(' '))
                    {
                        LoadBPM(str);
                    }
                    //----- bpm field end -----//


                    //----- stop field start -----//
                    else if (str.Contains("#STOP"))
                    {
                        LoadSTOP(str);
                    }
                    //----- stop field end -----//


                    else if (str.Contains("#LNOBJ ") || str.Contains("#lnobj "))
                    {
                        CurrentLongNoteType = LongNoteType.LNOBJ;
                        LNOBJValue = str.Substring(7);
                    }
                    else if (str.Contains("#LNTYPE ") || str.Contains("#lntype "))
                    {
                        int lntype_number = Int32.Parse(str.Substring(8));
                        if (lntype_number == 1)
                        {
                            CurrentLongNoteType = LongNoteType.LNTYPE1;
                        }
                        else if (lntype_number == 2)
                        {
                            CurrentLongNoteType = LongNoteType.LNTYPE2;
                        }
                    }
                }
            }

            sr.Close();
            fs.Close();
        }

        for (int i = nodeList.Count - 1; i > 0; --i)
        {
            if (nodeList.ElementAt(i).Value.noteList.Count == 0
                && nodeList.ElementAt(i).Value.eventList.Count == 0)
            {
                nodeList.Remove(nodeList.ElementAt(i).Key);
            }
            else
            {
                //Console.WriteLine("fuck");
                break;
            }
        }

        double currentBPM = BPM;
        double test = 0.0;
        for (int i = 0; i < nodeList.Count; i++)
        {
			// sort events
            List<Event> currentEventList = nodeList.ElementAt(i).Value.eventList;
            double currentNodeScale = nodeList.ElementAt(i).Value.scale;
            currentEventList.Sort(
                delegate(Event e1, Event e2)
                {
                    if (e1.calcedSequence > e2.calcedSequence) { return 1; }
                    else if (e1.calcedSequence < e2.calcedSequence) { return -1; }
                    else
                    {
                        if (e1.type > e2.type) { return 1; }
                        else if (e1.type < e2.type) { return -1; }
                        else { return 0; }
                    }
                });
			
			double nodeCount = currentNodeScale;
			test = currentNodeScale;
			double seqWithCalc = 0.0;
			for (int j = 0; j < currentEventList.Count; j++)
			{
				if (currentEventList.ElementAt(j).type == 3)
                {
                    double portion = (currentEventList.ElementAt(j).calcedSequence - seqWithCalc) * currentNodeScale;
                    seqWithCalc = currentEventList.ElementAt(j).calcedSequence;

                    SongLength += (portion / (currentBPM / 60 / 4));
                    int converted_value = Convert.ToInt32(currentEventList.ElementAt(j).value, 16);
                    currentBPM = converted_value;

                    nodeCount = Math.Max(0.0, nodeCount - portion);
                }
                else if (currentEventList.ElementAt(j).type == 8)
                {
                    double portion = (currentEventList.ElementAt(j).calcedSequence - seqWithCalc) * currentNodeScale;
                    seqWithCalc = currentEventList.ElementAt(j).calcedSequence;

                    SongLength += (portion / (currentBPM / 60 / 4));
                    currentBPM = BPMDic[currentEventList.ElementAt(j).value];

                    nodeCount = Math.Max(0.0, nodeCount - portion);
                }
                else if (currentEventList.ElementAt(j).type == 9)
                {
                    double time = stopDic[currentEventList.ElementAt(j).value];
                    //nodeCount += time / 192.0;
                    SongLength += ((time / 192.0) / (currentBPM / 60 / 4));
                }
            }

            SongLength += (nodeCount / (currentBPM / 60 / 4));
        }

        // add last null node 
        //currentBPM = info.bpm;
        //node_list.Add(node_list.Count, new Node());
        //node_list.Add(node_list.Count, new Node());

        SongLength += (2.0 / (BPM / 60 / 4));

        if (DivideMax < 64)
        {
            DivideMax = 64;
        }

        IsFinishLoadMainData = true;
    }
    #endregion

    #region LoadWAV
    private void LoadWAV(string str)
    {
        string wavId = str.Substring(4, 2);
        string wavFileName = str.Substring(7);

        string filePathBuilder = FolderPath + "/" + wavFileName;

        // check file exists
        string result = filePathBuilder;
		if (File.Exists(filePathBuilder) == false)
        {
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(wavFileName);
            string ext = Path.GetExtension(wavFileName);

            if (ext == ".wav")
            {
                string filePathWithoutExt = FolderPath + "/" + fileNameWithoutExt;
                if (File.Exists(filePathWithoutExt + ".ogg") == true)
                {
                    result = filePathWithoutExt + ".ogg";
                }
				else if (File.Exists(filePathWithoutExt + ".mp3") == true)
                {
                    result = filePathWithoutExt + ".mp3";
                }
            }
            else if (ext == ".ogg")
            {
                string filePathWithoutExt = FolderPath + "/" + fileNameWithoutExt;
				if (File.Exists(filePathWithoutExt + ".wav") == true)
                {
                    result = filePathWithoutExt + ".wav";
                }
				else if (File.Exists(filePathWithoutExt + ".mp3") == true)
                {
                    result = filePathWithoutExt + ".mp3";
                }
            }
            else if (ext == ".mp3")
            {
                string filePathWithoutExt = FolderPath + "/" + fileNameWithoutExt;
				if (File.Exists(filePathWithoutExt + ".ogg") == true)
                {
                    result = filePathWithoutExt + ".ogg";
                }
				else if (File.Exists(filePathWithoutExt + ".wav") == true)
                {
                    result = filePathWithoutExt + ".wav";
                }
            }
        }

        wavDic[wavId] = result;
    }
    #endregion

    #region LoadBMP
    private void LoadBMP(string str)
    {
        BMPCount++;

        string bmpIdStr = str.Substring(4, 2);
        string bmpFileName = str.Substring(7);

        string filePathBuilder = FolderPath + "\\" + bmpFileName;

        // check file exists
        string result = filePathBuilder.ToString();

        string ext = Path.GetExtension(bmpFileName);

        if (File.Exists(filePathBuilder) == false)
        {
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(bmpFileName);

            if (ext == ".bmp" || ext == ".jpg" || ext == ".png")
            {
                string filePathWithoutExt = FolderPath + "\\" + fileNameWithoutExt;
                if (File.Exists(filePathWithoutExt + ".jpg") == true)
                {
                    result = filePathWithoutExt + ".jpg";
                }
                else if (File.Exists(filePathWithoutExt + ".bmp") == true)
                {
                    result = filePathWithoutExt + ".bmp";
                }
                else if (File.Exists(filePathWithoutExt + ".png") == true)
                {
                    result = filePathWithoutExt + ".png";
                }
            }
            else if (ext == ".mpg" || ext == ".avi" || ext == ".wmv")
            {
                string filePathWithoutExt = FolderPath + "\\" + fileNameWithoutExt;
                if (File.Exists(filePathWithoutExt + ".mpg") == true)
                {
                    result = filePathWithoutExt + ".mpg";
                }
                else if (File.Exists(filePathWithoutExt + ".avi") == true)
                {
                    result = filePathWithoutExt + ".avi";
                }
                else if (File.Exists(filePathWithoutExt + ".wmv") == true)
                {
                    result = filePathWithoutExt + ".wmv";
                }
            }
        }

        // fix on PC (because PC fucking shit on "/")
        result = result.Replace("/", "\\");

        bmpDic[bmpIdStr] = result;
    }
    #endregion

    #region LoadBPM
    private void LoadBPM(string str)
    {
        string bpmIdStr = str.Substring(4, 2);
        double bpmNumber = double.Parse(str.Substring(7));
		//if(bpmNumber > 255)
		{
			//bpmNumber = 255;
		}

        // add bmp to dictionary
        BPMDic[bpmIdStr] = bpmNumber;
    }
    #endregion

    #region LoadSTOP
    private void LoadSTOP(string str)
    {
        string stopValueStr = str.Substring(5, 2);
        double stopNumber = double.Parse(str.Substring(8));

        // add bmp to dictionary
        stopDic[stopValueStr] = stopNumber;
    }
    #endregion
}
