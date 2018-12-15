using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class FadeImage : MonoBehaviour
{
    public Image uiImage;

    public Texture2D texture;

    public Texture2D texMask;

    //[Range(0, 3)]
    //public float fadeinTime = 0.4f;
    public float fadeoutTime = 1.4f;


    private bool bIsProcess = false;

    private float goalTime;
	private float time;
	private Material material;
	private bool fadein = true;
	private Action action;

	private static readonly string cutoff = "_Cutoff";
	private static readonly string mainTex = "_MainTex";
	private static readonly string maskTex = "_MaskTex";

    void OnEnable()
	{
		//GetComponent<Camera>().enabled = true;
	}

	void OnDisable()
	{
		//if(! fadein  )
			//GetComponent<Camera>().enabled = false;
	}

    void Awake()
    {
        material = uiImage.material;
    }

	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UpdateTexture(texture);
            UpdateMaskTexture(texMask);
            FadeOut(fadeoutTime, () =>
            {
                //FadeImage.Instance.UpdateMaskTexture(startMask);
                //FadeImage.Instance.FadeOut (fadeoutTime , null);
            });
        }

        if (bIsProcess == false)
            return;

        if ( goalTime < Time.time)
		{
            //targetRender.gameObject.SetActive(fadein);
            //enabled = false;
            bIsProcess = false;

            if ( action != null)
				action();
		}

		float diff = goalTime - Time.time;
		if( fadein )
		{
			float rate = (diff / time) * 2;
			material.SetFloat(cutoff, rate - 1);
		}
        else
        {
			float rate = 2- (diff / time) * 2;
			material.SetFloat(cutoff, rate - 1);
		}
	}

	public bool fading
	{
		get
        {
			return goalTime > Time.time;
		}
	}

	public void UpdateTexture(Texture texture)
	{
		material.SetTexture(mainTex, texture);
	}

	public void UpdateMaskTexture(Texture texture)
	{
		material.SetTexture(maskTex, texture);
	}

	public void FadeOut(float requestTime, Action act)
	{
		fadein = true;
		TimerSetup(requestTime, act);
	}

	public void FadeIn(float requestTime, Action act)
	{
		fadein = false;
		TimerSetup(requestTime, act);
	}

	void TimerSetup(float requestTime, Action act)
	{
		//targetRender.gameObject.SetActive(true);
		action = act;
		time = requestTime;
		goalTime = Time.time + time;
		//enabled = true;
        bIsProcess = true;
    }
}

