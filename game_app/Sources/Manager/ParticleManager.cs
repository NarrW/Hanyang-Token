using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// 정해놓은 갯수로 돌려쓰는 재활용 파티클
public enum E_PARTICLE_INSTANT_EFFECT
{
    RP_JUDGE_PGREAT = 0,
    RP_JUDGE_GREAT,
    RP_JUDGE_GOOD,
    RP_JUDGE_BAD,
    RP_JUDGE_POOR,
    RP_EXP,
}

// 정적인 파티클
public enum E_PARTICLE_STATIC_EFFECT
{
    RP_EXP_LONG = 0,
}

public class ParticleManager : MonoBehaviour
{
	private static ParticleManager m_Instance;
	public static ParticleManager Instance  
	{  
		get
		{
			if( m_Instance == null )  
			{  
				m_Instance =  FindObjectOfType(typeof(ParticleManager))as ParticleManager;
				if( m_Instance == null ) 
				{
					GameObject container = new GameObject();  
					container.name = "ParticleManager"; 
					m_Instance = container.AddComponent(typeof(ParticleManager)) as ParticleManager;
				}
			}
			return m_Instance;
		}
	}
	
	void Awake()
	{
		DontDestroyOnLoad (this);
	}
	
	void OnApplicationQuit()
	{
		m_Instance = null;
	}

    public int SelectState = 0;
  
    public class EffectSet
    {
        public List<ParticleSystem> m_listParticleSystem;
        public int m_nIdx;

        public EffectSet()
        {
            m_listParticleSystem = new List<ParticleSystem>();
            m_nIdx = 0;
        }
    }

    public Dictionary<E_PARTICLE_INSTANT_EFFECT, EffectSet> m_dicEffect;
    public Dictionary<E_PARTICLE_STATIC_EFFECT, EffectSet> m_dicStaticEffect;

    public void AddInstantEffect(E_PARTICLE_INSTANT_EFFECT eId, string sResourceName, int nCount)
    {
        if (m_dicEffect.ContainsKey(eId) == false)
        {
            m_dicEffect.Add(eId, new EffectSet());
            for (int i = 0; i < nCount; i++)
            {
                GameObject obj = GameObject.Instantiate(Resources.Load(sResourceName), new Vector3(0.0f, -1000.0f, 0.0f), Quaternion.identity) as GameObject;
				obj.isStatic = true;
                obj.transform.parent = m_objParticleEffectGroup.transform;
                m_dicEffect[eId].m_listParticleSystem.Add(obj.GetComponent<ParticleSystem>());
            }
        }
    }

    public void AddStaticEffect(E_PARTICLE_STATIC_EFFECT eId, string sResourceName, int nCount)
    {
        if (m_dicStaticEffect.ContainsKey(eId) == false)
        {
            m_dicStaticEffect.Add(eId, new EffectSet());
            for (int i = 0; i < nCount; i++)
            {
                GameObject obj = GameObject.Instantiate(Resources.Load(sResourceName), new Vector3(0.0f, -1000.0f, 0.0f), Quaternion.identity) as GameObject;
                obj.isStatic = true;
                obj.transform.parent = m_objParticleEffectGroup.transform;
                m_dicStaticEffect[eId].m_listParticleSystem.Add(obj.GetComponent<ParticleSystem>());
            }
        }
    }

    public void SetStaticEffectPosition(E_PARTICLE_STATIC_EFFECT eId, int nIndex, Vector3 vWorldPos)
    {
        if (m_dicStaticEffect.ContainsKey(eId) == false)
        {
            return;
        }

        EffectSet cRes = m_dicStaticEffect[eId];
        List<ParticleSystem> listParticleSystem = cRes.m_listParticleSystem;

        listParticleSystem[nIndex].transform.position = vWorldPos;
    }

    public void PlayStaticEffect(E_PARTICLE_STATIC_EFFECT eId, int nIndex)
    {
        EffectSet cRes = m_dicStaticEffect[eId];
        List<ParticleSystem> listParticleSystem = cRes.m_listParticleSystem;

        listParticleSystem[nIndex].Play();
    }

    public void StopStaticEffect(E_PARTICLE_STATIC_EFFECT eId, int nIndex)
    {
        EffectSet cRes = m_dicStaticEffect[eId];
        List<ParticleSystem> listParticleSystem = cRes.m_listParticleSystem;

        listParticleSystem[nIndex].Stop();
    }


    public void PlayInstantEffect(Vector3 vPos, E_PARTICLE_INSTANT_EFFECT eId)
    {
        EffectSet cRes = m_dicEffect[eId];
        List<ParticleSystem> listParticleSystem = cRes.m_listParticleSystem;

        listParticleSystem[cRes.m_nIdx].transform.position = vPos;
        listParticleSystem[cRes.m_nIdx].Play();

        cRes.m_nIdx++;
        if (cRes.m_nIdx >= listParticleSystem.Count)
        {
            cRes.m_nIdx = 0;
        }
    }

    GameObject m_objParticleEffectGroup;

    public void Init()
    {
        if (m_dicEffect != null)
            m_dicEffect.Clear();

        if (m_dicStaticEffect != null)
            m_dicStaticEffect.Clear();

        m_dicEffect = new Dictionary<E_PARTICLE_INSTANT_EFFECT, EffectSet>();
        m_dicStaticEffect = new Dictionary<E_PARTICLE_STATIC_EFFECT, EffectSet>();

        m_objParticleEffectGroup = new GameObject("ParticleEffectGroup");
    }

    public void PlayJudgeEffect(float fPositionX, E_PARTICLE_INSTANT_EFFECT eJudge)
    {
        PlayInstantEffect(new Vector3(fPositionX, 1, 1), eJudge);

        if (eJudge != E_PARTICLE_INSTANT_EFFECT.RP_JUDGE_POOR)
        {
            PlayInstantEffect(new Vector3(fPositionX, 0, 0), E_PARTICLE_INSTANT_EFFECT.RP_EXP);
        }
    }
}
