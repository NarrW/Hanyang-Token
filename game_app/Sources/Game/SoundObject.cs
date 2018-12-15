using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SoundObject
{
    public int handle;

    public int channel;

    public bool is_sample;

    public SoundObject(int _handle, int _channel, bool _is_sample)
    {
        handle = _handle;

        channel = _channel;

        is_sample = _is_sample;
    }
}