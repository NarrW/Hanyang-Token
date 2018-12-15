using UnityEngine;
using System.Collections;

public class BMSNote : MonoBehaviour
{
    // 일반노트, 롱노트 구분
    public E_NOTE_TYPE m_eNoteType;

    // 이벤트 인지 키 인지
    public E_CHANNEL_TYPE m_eChannelType;

    // 이벤트 노트의 경우
    public E_EVENT_CHANNEL m_eEventChannel;

    //!< Note 관련 변수 관련
    public bool m_bActive;                  //!< false 면 전체 업데이트 false
    public bool m_bIsShow;					
    public bool m_bIsKilled;
    public bool m_bSong;					//!< 체크 했는지
    public bool m_bIsPlayArea;              // 플레이 영역 안에 들어왔는지 여부
    public bool m_bIsPassedJudgeLine;
    public bool m_bIsSeperator = false;

    public string m_sIndex;					//!< 정보 값
    public int m_nKeyIndex;				    //!< 키보드 값
    public int m_nNoteKey;					//!< 노트 값
    public int m_nSoundNum;				    //!< 음 아이디 값

    public float m_fBpm;					//!< 노트의 BPM 정보
    public float m_fStop;					//!< 노트의 STOP 정보
    public float m_fCanselTime;			    //!< 변속 감속 시간 정보
    public float m_fDestTime;				//!< 목적지 까지 떨어지는 시간

    public float m_dBarperSecond;			//!< BPM 속도로 인해 한 마디당 내려오는 시간
    public float m_dBarDownTime;			//!< 프레임당 노트가 내려오는 시간

    public bool m_bIsVideo;                 // BGA 동영상인지 여부
    public Texture2D m_texBGA;              // BGA 이미지 텍스쳐

    public Vector3 m_vPos; // 내부적인 포지션값

    public SpriteRenderer m_compRenderer;

    public BMSSection m_cSection;

    public GameObject m_objLongBody;
    public SpriteRenderer m_compRendererLongBody;
    public float m_fLongBodyInitialScale;

    public virtual void Start() { }
    public virtual void Update() { }

    public virtual void PlaySound() { }
    public virtual void Kill() { }
    public virtual void SemiKill() { }

    public virtual void Judge() { }
}
