using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KeySelect : MonoBehaviour
{
    public KeySelectItem[] m_uiKeyBtn;

    [HideInInspector]
    public KeySelectItem m_uiPrevSelected;


    public int m_nSelectedIndex;

    void Start ()
    {
        // 기본 5키 선택
        m_nSelectedIndex = 0;
        m_uiKeyBtn[0].Selected();
        m_uiPrevSelected = m_uiKeyBtn[0];
    }

	void Update ()
    {
	    if(Input.GetKeyDown(KeyCode.F5))
        {
            SelectKey(0);
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            SelectKey(1);
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            SelectKey(2);
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            SelectKey(3);
        }
    }

    public void SelectKey(int nIndex)
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
    }
}
