using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour,IPanel
{
    Button BackButton;

    private void Awake()
    {
        BackButton = this.transform.Find("BackButton").GetComponent<Button>();
    }

    private void Start()
    {
        BackButton.onClick.AddListener(OnBackButtonClick);
    }

    public void OnEnter()
    {
        this.gameObject.SetActive(true);
    }

    public void OnExit()
    {
        this.gameObject.SetActive(false);
    }

    public void OnPause()
    {
        throw new System.NotImplementedException();
    }

    void OnBackButtonClick()
    {
        GameManager.Instance.UIManager.PopPanel();
    }
}
