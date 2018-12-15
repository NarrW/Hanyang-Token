using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class SongSelect : MonoBehaviour
{
    public GameObject m_objSongList;

    //public SelectedPanel m_uiSelectedPanel;

    public GameObject m_objLoadingCover;

    public TMPro.TextMeshProUGUI m_txtSongAmount;

    private VideoClip[] BGVideos;
    public VideoPlayer BGPlayer;


    public UnityEngine.UI.Text txtSelectedTitle;
    public UnityEngine.UI.Text txtSelectedArtist;
    public UnityEngine.UI.Text txtSelectedGenre;
    public UnityEngine.UI.Text txtSelectedLevel;

    public UnityEngine.UI.Text txtSelectedTime;
    public UnityEngine.UI.Text txtSelectedBPM;
    public UnityEngine.UI.Text txtSelectedNote;

    public UnityEngine.UI.Text txtSelectedHMT;


    public float m_fVelocityInterp = 0.0f;
    public int DestTIME = 0;
    public int CurTIME = 0;
    public int DestBPM = 0;
    public int CurBPM = 0;
    public int DestNOTE = 0;
    public int CurNOTE = 0;

    private int OverlayStatus = 0;

    public UnityEngine.UI.Text WalletHMTWonText;

    public UnityEngine.UI.Text HMTAmountText;
    public UnityEngine.UI.Slider HMTSlider;
    public float HMTAmount = 1000000.0f;
    private float HMTTransactionCurrentValue = 0.0f;

    public void HMTSliderControl()
    {
        HMTTransactionCurrentValue = HMTAmount * HMTSlider.value;

        string valStr = string.Format("{0}", HMTTransactionCurrentValue.ToString("n0"));
        string HMTTotalStr = string.Format("{0}", HMTAmount.ToString("n0"));
        HMTAmountText.text = valStr + " / " + HMTTotalStr + " HMT";
    }


    public Animator MainCanvasAnim;
    public Animator SelectCanvasAnim1;
    public Animator SelectCanvasAnim2;

    public RhythmVisualizator SoundBar;


    public GameObject PressObject;


    public GameObject WalletWindow;
    public GameObject TransactionWindow;
    private Animator WalletAnim;
    private Animator TransactionAnim;

    public GameObject Arrow1;
    public GameObject Arrow2;

    public GameObject Status;
    public GameObject Trans;

    public Text HMTTokenText;
    public Text WalletHMTTransAmountText;

    public GameObject TxContentNone;
    public GameObject TxContentOn;
    public Text TxDate;


    public GameObject NoWalletFoundObj;
    public GameObject WalletDetectObj;
    public GameObject WalletInformObj;
    public GameObject ScanWalletLoading;
    public GameObject ScanWalletButton;
    private bool isWalletDetected = false;

    public void ScanWallet()
    {
        StartCoroutine(Coroutine_ScanWallet());
    }

    IEnumerator Coroutine_ScanWallet()
    {
        ScanWalletLoading.SetActive(true);
        ScanWalletButton.SetActive(false);

        yield return new WaitForSeconds(5.0f);

        if(isWalletDetected == true)
        {
            NoWalletFoundObj.SetActive(false);
            WalletDetectObj.SetActive(true);
        }
        else
        {
            ScanWalletLoading.SetActive(false);
            ScanWalletButton.SetActive(true);
        }

        yield return null;
    }

    public InputField PasswordInput;
    public GameObject PasswordIncorrect;
    public GameObject UnlockButton;
    public GameObject UnlockLoading;

    public GameObject StatusLoading;
    public GameObject TransLoading;

    public void Unlock()
    {
        if (PasswordInput.text == "KOrkdgur05!")
        {
            PasswordIncorrect.SetActive(false);
            StartCoroutine(Coroutine_Unlock());
        }
        else
        {
            PasswordIncorrect.SetActive(true);
        }
    }

    IEnumerator Coroutine_Unlock()
    {
        UnlockButton.SetActive(false);
        UnlockLoading.SetActive(true);

        yield return new WaitForSeconds(5.0f);

        UnlockLoading.SetActive(false);
        WalletDetectObj.SetActive(false);
        WalletInformObj.SetActive(true);
        StatusLoading.SetActive(true);

        yield return new WaitForSeconds(2.0f);

        StatusLoading.SetActive(false);
        Status.SetActive(true);

        TransLoading.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        TransLoading.SetActive(false);
        Trans.SetActive(true);

        yield return null;
    }

    public void OpenWalletWindow()
    {
        if (OverlayStatus != 0)
            return;

        Arrow2.SetActive(false);

        OverlayStatus = 1;

        HMTTokenText.text = string.Format("{0}", HMTAmount.ToString("n0"));
        WalletHMTTransAmountText.text = string.Format("{0}", HMTTransactionCurrentValue.ToString("n0"));
        //WalletHMTWonText.text = string.Format("{0}", HMTAmount.ToString("n0")) + " Won";
        WalletAnim.SetTrigger("WalletIn");
    }


    public GameObject ConfirmWindow;
    private Animator ConfirmAnim;
    public Text ConfirmText;
    public GameObject TransactionFadeCover;
    public GameObject TransactionLoading;
    public GameObject TransactionButton;

    public void Transfer()
    {
        TransactionFadeCover.SetActive(true);

        ConfirmText.text = "Are you sure you want to withdraw " + string.Format("{0}", HMTTransactionCurrentValue.ToString("n0")) + " HMT?";
        ConfirmWindow.SetActive(true);
        ConfirmAnim.SetTrigger("ConfirmIn");
    }

    public void OpenTransactionWindow()
    {
        if (OverlayStatus != 0)
            return;

        Arrow1.SetActive(false);

        OverlayStatus = 2;

        TransactionAnim.SetTrigger("TransactionIn");
    }

    public void Confirm_OK()
    {
        StartCoroutine(Coroutine_Confirm_OK());
    }

    IEnumerator Coroutine_Confirm_OK()
    {
        TransactionFadeCover.SetActive(false);

        HMTAmount -= HMTTransactionCurrentValue;

        ConfirmAnim.SetTrigger("ConfirmOut");
        TransactionButton.SetActive(false);
        TransactionLoading.SetActive(true);

        yield return new WaitForSeconds(10.0f);

        OverlayStatus = 0;
        TransactionAnim.SetTrigger("TransactionOut");
        TxDate.text = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        TxContentOn.SetActive(true);
        TxContentNone.SetActive(true);

        yield return new WaitForSeconds(0.8f);

        OpenWalletWindow();

        yield return new WaitForSeconds(1.0f);
        TransactionButton.SetActive(true);
        TransactionLoading.SetActive(false);

        yield return null;
    }

    public void Confirm_Cancel()
    {
        TransactionFadeCover.SetActive(false);

        ConfirmAnim.SetTrigger("ConfirmOut");
    }




    [HideInInspector]
    public SongSelectItem[] m_uiSongPanel;

    [HideInInspector]
    public SongSelectItem m_uiCurrentSongPanel;

    [HideInInspector]
    public Vector3[] m_vecDestPos;

    [HideInInspector]
    public int m_nSelectedIndex;

    void Awake()
    {
        m_vecDestPos = new Vector3[5];
        m_vecDestPos[0] = new Vector3(0, 800, 0);
        m_vecDestPos[1] = new Vector3(0, 400, 0);
        m_vecDestPos[2] = new Vector3(0, 0, 0);
        m_vecDestPos[3] = new Vector3(0, -400, 0);
        m_vecDestPos[4] = new Vector3(0, -800, 0);

        m_nSelectedIndex = 0;
    }

    void Start ()
    {
        WalletAnim = WalletWindow.GetComponent<Animator>();
        TransactionAnim = TransactionWindow.GetComponent<Animator>();
        ConfirmAnim = ConfirmWindow.GetComponent<Animator>();

        BGVideos = new VideoClip[10];
        BGVideos[0] = Resources.Load<VideoClip>("181208/clock");
        BGVideos[1] = Resources.Load<VideoClip>("181208/fake");
        BGVideos[2] = Resources.Load<VideoClip>("181208/floating");
        BGVideos[3] = Resources.Load<VideoClip>("181208/madeon");
        BGVideos[4] = Resources.Load<VideoClip>("181208/sakura");
        BGVideos[5] = Resources.Load<VideoClip>("181208/taeyeon");
        BGVideos[6] = Resources.Load<VideoClip>("181208/tether");
        BGVideos[7] = Resources.Load<VideoClip>("181208/twice");

        m_txtSongAmount.text = "1/" + DataManager.Instance.m_dicBMS.Count.ToString();

        for (int i=0; i<5; i++)
        {
            GameObject newObj = GameObject.Instantiate(Resources.Load("UI/Select/SongPanel"), Vector3.zero, Quaternion.identity) as GameObject;
            newObj.transform.parent = m_objSongList.transform;
            newObj.transform.localScale = new Vector3(1, 1, 1);
            newObj.transform.localRotation = Quaternion.identity;

            m_uiSongPanel[i] = newObj.GetComponent<SongSelectItem>();
            m_uiSongPanel[i].m_nCurrentPositionIndex = i;
            m_uiSongPanel[i].m_nCurrentBMSIndex = i - 2;
            m_uiSongPanel[i].SetTargetPosition(m_vecDestPos[m_uiSongPanel[i].m_nCurrentPositionIndex]);

            m_uiSongPanel[i].UpdateInform();

            if (i==2)
            {
                m_uiCurrentSongPanel = m_uiSongPanel[i];

                m_uiSongPanel[i].m_animator.SetTrigger("MoveFront");
                //m_uiSelectedPanel.SetSelectedInform(m_uiCurrentSongPanel.cCurrentBMS);
            }
        }

        UpdateInform();

        if (ParticleManager.Instance.SelectState == 1)
        {
            ParticleManager.Instance.SelectState = 0;
            CurrentSceneState = 2;
            StartCoroutine(Coroutine_FadeMain());
        }
    }

    private int CurrentSceneState = 0;

    IEnumerator Coroutine_FadeMain()
    {
        MainCanvasAnim.SetTrigger("Fade");
        SelectCanvasAnim1.SetTrigger("Trig");

        yield return new WaitForSeconds(0.6f);

        SoundBar.SetActiveBars(false);
        SelectCanvasAnim2.SetTrigger("Trig");

        yield return null;
    }

    void Update ()
    {
        if (CurrentSceneState == 0)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                PressObject.SetActive(true);
                CurrentSceneState++;
            }
        }
        else if (CurrentSceneState == 1)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartCoroutine(Coroutine_FadeMain());
                CurrentSceneState++;
            }   
        }
        else if (CurrentSceneState == 2)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (OverlayStatus == 0)
                {
                    StartCoroutine(Coroutine_LoadLevel());
                }
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                BGPlayer.clip = BGVideos[m_nSelectedIndex];
                BGPlayer.time = 60.0;
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                OpenWalletWindow();
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                OpenTransactionWindow();
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                isWalletDetected = true;
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                isWalletDetected = false;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (OverlayStatus == 1)
                {
                    WalletAnim.SetTrigger("WalletOut");
                }
                else if (OverlayStatus == 2)
                {
                    TransactionAnim.SetTrigger("TransactionOut");
                }

                OverlayStatus = 0;
                Arrow1.SetActive(true);
                Arrow2.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ScrollNext();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                ScrollPrev();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ScrollLeft();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ScrollRight();
            }


            CurTIME = (int)Mathf.Lerp(CurTIME, DestTIME, Time.deltaTime * 10);
            CurBPM = (int)Mathf.Lerp(CurBPM, DestBPM, Time.deltaTime * 10);
            CurNOTE = (int)Mathf.Lerp(CurNOTE, DestNOTE, Time.deltaTime * 10);


            int min = CurTIME / 60;

            string secStr = "";
            int sec = CurTIME % 60;
            if (sec < 10)
            {
                secStr = "0" + sec;
            }
            else
            {
                secStr = sec.ToString();
            }

            txtSelectedTime.text = min.ToString() + ":" + secStr;
            txtSelectedBPM.text = CurBPM.ToString();
            txtSelectedNote.text = CurNOTE.ToString();
        }
    }

    IEnumerator Coroutine_LoadLevel()
    {
        m_objLoadingCover.SetActive(true);
        GameManager.Instance.m_cCurrentPlayingBMS = m_uiCurrentSongPanel.cCurrentBMS;
        AsyncOperation async = SceneManager.LoadSceneAsync("Game");

        while(!async.isDone)
        {
            yield return null;

        }
    }

    public void ScrollLeft()
    {
        if (m_uiCurrentSongPanel.m_nCurrentDifficultyIndex == 0)
            return;

        m_uiCurrentSongPanel.m_nCurrentDifficultyIndex -= 1;
        m_uiCurrentSongPanel.UpdateInform();
        m_uiCurrentSongPanel.ImageFillLeft();

        //m_uiSelectedPanel.SetSelectedInform(m_uiCurrentSongPanel.cCurrentBMS);
    }

    public void ScrollRight()
    {
        if (m_uiCurrentSongPanel.m_nCurrentDifficultyIndex >= DataManager.Instance.m_dicBMS.ElementAt(m_uiCurrentSongPanel.m_nCurrentBMSIndex).Value.Count - 1)
            return;

        m_uiCurrentSongPanel.m_nCurrentDifficultyIndex += 1;
        m_uiCurrentSongPanel.UpdateInform();
        m_uiCurrentSongPanel.ImageFillRight();

        //m_uiSelectedPanel.SetSelectedInform(m_uiCurrentSongPanel.cCurrentBMS);
    }

    public void ScrollPrev()
    {
        if (m_nSelectedIndex == 0)
            return;

        m_uiCurrentSongPanel.m_nCurrentDifficultyIndex = 0;

        foreach (SongSelectItem item in m_uiSongPanel)
        {
            if (item.m_nCurrentPositionIndex == 2)
            {
                item.m_animator.SetTrigger("MoveBack");
            }

            item.m_nCurrentPositionIndex++;

            if (item.m_nCurrentPositionIndex == 2)
            {
                m_uiCurrentSongPanel = item;

                //m_uiSelectedPanel.SetSelectedInform(m_uiCurrentSongPanel.cCurrentBMS);

                item.m_animator.SetTrigger("MoveFront");
            }

            if (item.m_nCurrentPositionIndex > 4)
            {
                item.m_nCurrentPositionIndex = 0;
                item.SetCurrentPosition(m_vecDestPos[item.m_nCurrentPositionIndex]);

                // set bms
                item.m_nCurrentBMSIndex = m_nSelectedIndex - 3;
                item.UpdateInform();
            }

            item.SetTargetPosition(m_vecDestPos[item.m_nCurrentPositionIndex]);
        }

        m_nSelectedIndex -= 1;

        m_txtSongAmount.text = (m_nSelectedIndex + 1) + "/" + DataManager.Instance.m_dicBMS.Count.ToString();

        UpdateInform();
    }

    public void ScrollNext()
    {
        if (DataManager.Instance.m_dicBMS.Count - 1 <= m_nSelectedIndex)
            return;

        m_uiCurrentSongPanel.m_nCurrentDifficultyIndex = 0;

        foreach (SongSelectItem item in m_uiSongPanel)
        {
            if (item.m_nCurrentPositionIndex == 2)
            {
                item.m_animator.SetTrigger("MoveBack");
            }

            item.m_nCurrentPositionIndex--;

            if (item.m_nCurrentPositionIndex == 2)
            {
                m_uiCurrentSongPanel = item;

                //m_uiSelectedPanel.SetSelectedInform(m_uiCurrentSongPanel.cCurrentBMS);

                item.m_animator.SetTrigger("MoveFront");
            }

            if (item.m_nCurrentPositionIndex < 0)
            {
                item.m_nCurrentPositionIndex = 4;
                item.SetCurrentPosition(m_vecDestPos[item.m_nCurrentPositionIndex]);
                
                // set bms
                item.m_nCurrentBMSIndex = m_nSelectedIndex + 3;
                item.UpdateInform();
            }

            item.SetTargetPosition(m_vecDestPos[item.m_nCurrentPositionIndex]);
        }

        m_nSelectedIndex += 1;

        m_txtSongAmount.text = (m_nSelectedIndex + 1) + "/" + DataManager.Instance.m_dicBMS.Count.ToString();

        UpdateInform();
    }

    public void UpdateInform()
    {
        txtSelectedTitle.text = m_uiCurrentSongPanel.cCurrentBMS.m_cBMSInfo.sTitle;
        txtSelectedArtist.text = m_uiCurrentSongPanel.cCurrentBMS.m_cBMSInfo.sArtist;
        txtSelectedGenre.text = m_uiCurrentSongPanel.cCurrentBMS.m_cBMSInfo.sGenre;
        txtSelectedLevel.text = m_uiCurrentSongPanel.cCurrentBMS.m_cBMSInfo.sPlaylevel;

        DestTIME = m_uiCurrentSongPanel.cCurrentBMS.m_nLength;
        DestBPM = int.Parse(m_uiCurrentSongPanel.cCurrentBMS.m_cBMSInfo.sBpm);
        DestNOTE = m_uiCurrentSongPanel.cCurrentBMS.m_nTotalNote;
    }

    /*public void SelectKey(int nIndex)
    {
        if (m_nSelectedIndex == nIndex)
            return;

        // 이전 선택된 애니메이션 중지
        m_uiPrevSelected.Deselected();

        // 현재 선택된 키로
        m_nSelectedIndex = nIndex;
        m_uiPrevSelected = m_uiKeyBtn[m_nSelectedIndex];

        // 선택 연출
        m_uiKeyBtn[m_nSelectedIndex].Selected();
    }*/
}
