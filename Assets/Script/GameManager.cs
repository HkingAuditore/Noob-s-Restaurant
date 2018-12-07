using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // Use this for initialization
    //void Start () {
    //       Cursor.visible = false;
    //}

    SceneStateManager SceneStateManager;
    UIManager UIManager;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneStateManager = new SceneStateManager(this);
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
