using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public SceneStateManager sceneStateManager;
    public UIManager uiManager;
    public ParticleSystemManager particleSystemManager;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        sceneStateManager = new SceneStateManager();
        uiManager = new UIManager();
        particleSystemManager = new ParticleSystemManager();
    }

    private void Start()
    {
        sceneStateManager.Init();
        uiManager.Init();
        particleSystemManager.Init();
    }

    void Update()
    {
        sceneStateManager.UpdateState();
        uiManager.UpdateUI();
    }
}
