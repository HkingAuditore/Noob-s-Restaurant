using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>{

    public SceneStateManager sceneStateManager;
    public UIManager uiManager;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        sceneStateManager = new SceneStateManager();
        uiManager = new UIManager();
    }

    private void Start()
    {
        sceneStateManager.Init();
        uiManager.Init();
    }

    // Update is called once per frame
    void Update()
    {
        sceneStateManager.UpdateState();
        uiManager.UpdateUI();
    }
}
