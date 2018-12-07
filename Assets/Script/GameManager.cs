using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>{

    // Use this for initialization
    //void Start () {
    //       Cursor.visible = false;
    //}

    public SceneStateManager SceneStateManager;
    public UIManager UIManager;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);

        SceneStateManager = new SceneStateManager();
        UIManager = new UIManager();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SceneStateManager.UpdateState();
    }
}
