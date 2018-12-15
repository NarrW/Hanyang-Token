using UnityEngine;
using Un4seen.Bass;

using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

public class BassManager : MonoBehaviour
{
    private static BassManager m_Instance;
    public static BassManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType(typeof(BassManager)) as BassManager;
                if (m_Instance == null)
                {
                    GameObject container = new GameObject();
                    container.name = "BassManager";
                    m_Instance = container.AddComponent(typeof(BassManager)) as BassManager;
                }
            }
            return m_Instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void OnApplicationQuit()
    {
        m_Instance = null;

        Release();
    }

    int isInitComplete = 0;

    //[DllImport("libbass")]
    //public static extern bool BASS_Init(int device, int freq, BASSInit flags, IntPtr win);

    /*[DllImport("libbass")]
    public static extern int BASS_StreamCreateFile(string file, long offset, long length, BASSFlag flags);

    [DllImport("libbass")]
    public static extern bool BASS_ChannelPlay(int handle, bool restart);

    [DllImport("libbass")]
    public static extern bool BASS_ChannelStop(int handle);

    [DllImport("libbass")]
    public static extern bool BASS_ChannelSetPosition(int handle, long pos);

    [DllImport("libbass")]
    public static extern BASSError BASS_ErrorGetCode();*/

    public void Init()
    {
        string email = "over2ture@gmail.com";
        string key = "2X24172319152222";
        BassNet.Registration(email, key);
  
        isInitComplete = -1;

        if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
        {
            isInitComplete = 1;
            throw new System.Exception(String.Format("Unable to load bass audio library."));
        }

        isInitComplete = 2;
    }

    public void Release()
    {
        // free BASS
        Bass.BASS_Free();
    }

    public int CreateStream(string _path)
    {
        // create a stream channel from a file
        int stream = Bass.BASS_StreamCreateFile(_path, 0L, 0L, BASSFlag.BASS_SAMPLE_MONO);
        if (stream != 0)
        {
            return stream;
        }
        else
        {
            // error
            Console.WriteLine("Stream load error: {0}", Bass.BASS_ErrorGetCode());

            return 0;
        }
    }

    public void RemoveStream(int _stream)
    {
        // free the stream
        Bass.BASS_StreamFree(_stream);
    }

    public int CreateSample(string _path)
    {
        int stream = Bass.BASS_SampleLoad(_path, 0L, 0, 1, BASSFlag.BASS_SAMPLE_MONO);
        if (stream != 0)
        {
            return stream;
        }
        else
        {
            // error
            Console.WriteLine("Sample load error: {0}", Bass.BASS_ErrorGetCode());

            return 0;
        }
    }

    public void RemoveSample(int _stream)
    {
        Bass.BASS_SampleFree(_stream);
    }

    public void PlayStreamWithFade(int _stream, bool _restart)
    {
        if (_stream != 0)
        {
            Bass.BASS_ChannelSetAttribute(_stream, BASSAttribute.BASS_ATTRIB_VOL, 0.0f);
            Bass.BASS_ChannelSlideAttribute(_stream, BASSAttribute.BASS_ATTRIB_VOL, 1.0f, 1500);
            Bass.BASS_ChannelPlay(_stream, _restart);
        }
    }

    public void PlayStream(int _stream, bool _restart)
    {
        if (_stream != 0)
        {
            Bass.BASS_ChannelPlay(_stream, _restart);
        }
    }

    public void PauseStream(int _stream)
    {
        if (_stream != 0)
        {
            Bass.BASS_ChannelPause(_stream);
        }
    }

    public void StopStream(int _stream)
    {
        if (_stream != 0)
        {
            Bass.BASS_ChannelStop(_stream);
            Bass.BASS_ChannelSetPosition(_stream, 0);
        }
    }

    public void StopSample(int _stream)
    {
        if (_stream != 0)
        {
            Bass.BASS_SampleStop(_stream);
            Bass.BASS_ChannelSetPosition(Bass.BASS_SampleGetChannel(_stream, false), 0);
        }
    }

    public void SetPosition(int _stream, double _seconds)
    {
        Bass.BASS_ChannelSetPosition(_stream, _seconds);
    }

    public void SetPosition(int _stream, long _pos)
    {
        Bass.BASS_ChannelSetPosition(_stream, _pos);
    }

    // seconds
    public double GetPosition(int _stream)
    {
        long pos = Bass.BASS_ChannelGetPosition(_stream);
        return Bass.BASS_ChannelBytes2Seconds(_stream, pos);
    }

    // milliseconds
    public double GetPositionMS(int _stream)
    {
        long pos = Bass.BASS_ChannelGetPosition(_stream);
        double second = Bass.BASS_ChannelBytes2Seconds(_stream, pos);
        double result = second * 1000;
        return result;
    }

    public double GetDuration(int _stream)
    {
        long len = Bass.BASS_ChannelGetLength(_stream);
        return Bass.BASS_ChannelBytes2Seconds(_stream, len);
    }

    public BASSActive GetState(int _stream)
    {
        return Bass.BASS_ChannelIsActive(_stream);
    }

    public bool isPlaying(int _stream)
    {
        if (_stream != 0)
        {
            if (Bass.BASS_ChannelIsActive(_stream) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsOver(int _stream)
    {
        if (_stream != 0)
        {
            if (Bass.BASS_ChannelGetPosition(_stream) == Bass.BASS_ChannelGetLength(_stream))
            {
                return true;
            }
        }

        return false;
    }

    public void GetSpectrum(int _stream, out float[] _buffer)
    {
        _buffer = new float[256];
        Bass.BASS_ChannelGetData(_stream, _buffer, (int)(BASSData.BASS_DATA_FFT512 | BASSData.BASS_DATA_FFT_COMPLEX));
    }
}