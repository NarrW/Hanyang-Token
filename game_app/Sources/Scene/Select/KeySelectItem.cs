using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KeySelectItem : MonoBehaviour
{
    [HideInInspector]
    public RectTransform m_rectTrs;
    [HideInInspector]
    public Image m_image;
    [HideInInspector]
    public Animator m_animator;
    [HideInInspector]
    public GameObject m_objSelectedBorder; 

    void Start()
    {
        m_rectTrs = gameObject.GetComponent<RectTransform>();
        m_image = gameObject.GetComponent<Image>();
        m_animator = gameObject.GetComponent<Animator>();

        m_objSelectedBorder = transform.Find("selected_border").gameObject;
        //m_objSelectedBorder.SetActive(false);
    }

    public void Deselected()
    {
        //m_objSelectedBorder.SetActive(false);
        m_animator.SetTrigger("Normal");
    }

    public void Selected()
    {
        //m_objSelectedBorder.SetActive(true);
        m_animator.SetTrigger("Pressed");
    }
}
