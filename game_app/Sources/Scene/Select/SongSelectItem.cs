using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SongSelectItem : MonoBehaviour
{
    [HideInInspector]
    public RectTransform m_rectTrs;
    [HideInInspector]
    public Image m_image;
    [HideInInspector]
    public Animator m_animator;

    [HideInInspector]
    public Vector3 m_vecTargetPosition;
    private Vector3 velocity = Vector3.zero;

    [HideInInspector]
    public int m_nCurrentPositionIndex;
    [HideInInspector]
    public int m_nCurrentBMSIndex;
    [HideInInspector]
    public int m_nCurrentDifficultyIndex = 0;

    public BMSLoader cCurrentBMS;

    public bool m_bIsImageLoaded = false;

    public Image m_imgStagefile;
    public Animator m_animStagefile;

    public Text m_txtTitle;
    public Text m_txtArtist;
    public Text m_txtDifficulty;

    void Awake()
    {
        m_rectTrs = gameObject.GetComponent<RectTransform>();
        m_image = gameObject.GetComponent<Image>();
        m_animator = gameObject.GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        m_rectTrs.anchoredPosition3D = Vector3.SmoothDamp(m_rectTrs.anchoredPosition3D, m_vecTargetPosition, ref velocity, Time.deltaTime * 10);
    }

    public void SetTargetPosition(Vector3 vecTargetPos)
    {
        m_vecTargetPosition = vecTargetPos;
    }

    public void SetCurrentPosition(Vector3 vecCurrentPos)
    {
        m_rectTrs.anchoredPosition3D = vecCurrentPos;
    }

    public void ImageFillLeft()
    {
        //m_imgStagefile.fillOrigin = 0;
        //m_animStagefile.SetTrigger("Fill");
    }

    public void ImageFillRight()
    {
        //m_imgStagefile.fillOrigin = 1;
        //m_animStagefile.SetTrigger("Fill");
    }

    public void UpdateInform()
    {
        if (m_nCurrentBMSIndex < 0 || m_nCurrentBMSIndex >= DataManager.Instance.m_dicBMS.Count)
            return;

        cCurrentBMS = DataManager.Instance.m_dicBMS.ElementAt(m_nCurrentBMSIndex).Value[m_nCurrentDifficultyIndex];

        StopCoroutine(Coroutine_LoadImage(cCurrentBMS));
        StartCoroutine(Coroutine_LoadImage(cCurrentBMS));

        /*
        // set stage file
#if UNITY_ANDROID && !UNITY_EDITOR
                WWW www = new WWW("file://" + Application.dataPath + "/../" + media.m_sPath);
#else
        WWW www = new WWW("file://" + Application.dataPath + "/../" + cCurrentBMS.m_sFolderPath + "/" + cCurrentBMS.m_cBMSInfo.sStagefile);
#endif
        Texture2D texTmp = new Texture2D(4, 4, TextureFormat.DXT1, false);
        www.LoadImageIntoTexture(texTmp);

        Sprite sprite = Sprite.Create(texTmp, new Rect(0, 0, texTmp.width, texTmp.height), new Vector2(0.5f, 0.5f));
        m_imgStagefile.sprite = sprite;*/

        // set title with out details
        char[] seps = new char[] { '[' };
        string[] sTitleSplited = cCurrentBMS.m_cBMSInfo.sTitle.Split(seps, 2);
        m_txtTitle.text = sTitleSplited[0];

        // set artist
        m_txtArtist.text = cCurrentBMS.m_cBMSInfo.sArtist;

        // set difficulty
        m_txtDifficulty.text = cCurrentBMS.m_cBMSInfo.sPlaylevel;
    }

    IEnumerator Coroutine_LoadImage(BMSLoader cCurrentBMS)
    {
        // set stage file

#if UNITY_ANDROID && !UNITY_EDITOR
                WWW www = new WWW("file://" + Application.dataPath + "/../" + media.m_sPath);
#else
        WWW www = new WWW("file://" + Application.dataPath + "/../" + cCurrentBMS.m_sFolderPath + "/" + cCurrentBMS.m_cBMSInfo.sStagefile);
#endif
        Texture2D texTmp = new Texture2D(4, 4, TextureFormat.DXT1, false);
        www.LoadImageIntoTexture(texTmp);

        Sprite sprite = Sprite.Create(texTmp, new Rect(0, 0, texTmp.width, texTmp.height), new Vector2(0.5f, 0.5f));
        m_imgStagefile.sprite = sprite;

        yield return null;
    }
}
