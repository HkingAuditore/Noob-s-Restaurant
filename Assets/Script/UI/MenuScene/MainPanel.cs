using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel :MonoBehaviour,IPanel
{
    Button StartButton;
    Button SettingButton;
    Button QuitButton;

    private void Awake()
    {
        StartButton = this.transform.Find("StartButton").GetComponent<Button>();
        SettingButton = this.transform.Find("SettingButton").GetComponent<Button>();
        QuitButton = this.transform.Find("QuitButton").GetComponent<Button>();
    }

    private void Start()
    {
        StartButton.onClick.AddListener(OnStartButtonClick);
        SettingButton.onClick.AddListener(OnSettingButtonClick);
        QuitButton.onClick.AddListener(OnQuitButtonClick);
    }

    public void OnEnter()
    {
        this.gameObject.SetActive(true);
    }

    public void OnExit()
    {

    }

    public void OnPause()
    {
    }

    void OnStartButtonClick()
    {
        GameManager.Instance.SceneStateManager.SetState(new GameScene());
    }

    void OnSettingButtonClick()
    {
        Debug.Log("OnSettingButtonClick");
        GameManager.Instance.UIManager.PushPanel("SettingPanel");
    }

    void OnQuitButtonClick()
    {
        Application.Quit();
    }
}
