using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{

    public SceneStateManager sceneStateManager;
    public UIManager uiManager;
    public new ParticleSystem particleSystem;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        sceneStateManager = new SceneStateManager();
        uiManager = new UIManager();
        particleSystem = new ParticleSystem();
    }

    private void Start()
    {
        sceneStateManager.Init();
        uiManager.Init();
        particleSystem.Init();
    }

    void Update()
    {
        sceneStateManager.UpdateState();
        uiManager.UpdateUI();
    }
}
