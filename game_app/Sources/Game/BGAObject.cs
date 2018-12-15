using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BGAObject
{
    public Texture2D tex;
    public bool isVideo;

    public BGAObject(Texture2D _tex, bool _isVideo)
    {
        tex = _tex;
        isVideo = _isVideo;
    }
}