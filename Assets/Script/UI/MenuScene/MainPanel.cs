using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel :IPanel
{
    Canvas canvas;
    GameObject mainPanel;
    Button StartButton;
    Button RecipeButton;
    Button QuitButton;

    public string GetPanelName()
    {
        return "MainPanel";
    }

    public void OnEnter()
    {
        canvas = GameObject.FindObjectOfType<Canvas>();
        mainPanel = canvas.transform.Find("MainPanel").gameObject;
        StartButton = canvas.transform.Find("MainPanel/StartButton").GetComponent<Button>();
        RecipeButton = canvas.transform.Find("MainPanel/RecipeButton").GetComponent<Button>();
        QuitButton = canvas.transform.Find("MainPanel/QuitButton").GetComponent<Button>();
        mainPanel.SetActive(true);
        StartButton.onClick.AddListener(OnStartButtonClick);
        RecipeButton.onClick.AddListener(OnRecipeButtonClick);
        QuitButton.onClick.AddListener(OnQuitButtonClick);
    }

    public void OnExit()
    {
        StartButton.onClick.RemoveListener(OnStartButtonClick);
        RecipeButton.onClick.RemoveListener(OnRecipeButtonClick);
        QuitButton.onClick.RemoveListener(OnQuitButtonClick);
    }

    public void OnPause()
    {
    }

    void OnStartButtonClick()
    {
        GameManager.Instance.sceneStateManager.SetState(new GameScene());
    }

    void OnRecipeButtonClick()
    {
        GameManager.Instance.uiManager.PushPanel(new RecipePanel());
    }

    void OnQuitButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnUpdate()
    {
    }
}
