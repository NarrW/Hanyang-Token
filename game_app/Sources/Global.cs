using UnityEngine;
using System.Collections;

/// <summary>
/// 채널 타입
/// </summary>
public enum E_EVENT_CHANNEL
{
    E_CHANNEL_BGM = 1,		//!< 배경음 채널
    E_CHANNEL_NODESCALE,		//!< 마디 단축 채널
    E_CHANNEL_BPM,			//!< BPM 채널
    E_CHANNEL_BGA,			//!< BGA 채널
    E_CHANNEL_BM98EX,		//!< BM 98 확장 채널
    E_CHANNEL_POORBGA,		//!< poor BGA 채널
    E_CHANNEL_BGALAYER,			//!< BGA 레이어 채널
    E_CHANNEL_EX_BPM = 8,	//!< 확장 BPM 채널
    E_CHANNEL_STOP,			//!< 시퀸스 정지 채널
    E_CHANNEL_NONE,
    E_CHANNEL_NOTE,
    E_CHANNEL_NODELINE,
};

public enum E_CHANNEL_TYPE
{
    E_CHANNEL_TYPE_EVENT = 0,
    E_CHANNEL_TYPE_P1 = 1,
    E_CHANNEL_TYPE_P2 = 2,
    E_CHANNEL_TYPE_P3 = 3,
    E_CHANNEL_TYPE_P4 = 4,
    E_CHANNEL_TYPE_LN1 = 5,
    E_CHANNEL_TYPE_LN2 = 6,
}

/// <summary>
/// 키보드 타입
/// </summary>
public enum E_KEYBOARD_TYPE
{
    E_KEYBOARD_ONE = 1,
    E_KEYBOARD_TWO,
    E_KEYBOARD_THREE,
    E_KEYBOARD_FOUR,
    E_KEYBOARD_FIVE,
    E_KEYBOARD_SIX,
    E_KEYBOARD_TEMP,
    E_KEYBOARD_EIGHT,
    E_KEYBOARD_NINE,
    E_KEYBOARD_MAX,
};

/// <summary>
/// 키 개수
/// </summary>
public enum E_KEY_NUM
{
    E_KEY_5K = 1,
    E_KEY_7K,
};

/// <summary>
/// 노트 유형
/// </summary>
public enum E_NOTE_TYPE
{
    NORMAL,
    LONG_START,
    LONG_END
}

/// <summary>
/// 롱노트 방식
/// </summary>
public enum E_LONGNOTE_TYPE
{
    NONE,
    LNTYPE1,
    LNTYPE2,
    LNOBJ
}

public class Global
{
    public const float MAX_NOTEFIELD_HEIGHT  = 32.0f;    //!< 마디 높이
    public const float JUDGE_POSITIONZ = 0.0f;     //!< 판정 처리 위치

    // 판정 수치 값
    public const float fJudgePosPGreat  = 0.5f;
    public const float fJudgePosGreat   = 1.0f;
    public const float fJudgePosGood    = 1.5f;
    public const float fJudgePosBad     = 2.25f;
    public const float fJudgePosPoor    = 3.0f;
}
