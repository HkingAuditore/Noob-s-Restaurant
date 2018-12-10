using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStateManager
{
    IState state;
    bool isLoadFinish = true;

    public void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        state = new InitScene();
        state.OnStateStart();
    }

    public void UpdateState()
    {
        if (isLoadFinish)
        {
            state.OnStateUpdate();
        }
    }

    public void SetState(IState state)
    {
        switch (state.GetStateName())
        {
            //2个特殊场景（init,load）单独处理
            case "Init":
                Debug.LogWarning("Init场景用于创建gamemanager和初始化,不允许被二次加载");
                break;

            case "Load":
                Debug.LogWarning("load场景不允许被单独加载");
                break;

            case "Game":
                LoadScene(new LoadScene(), state);
                break;

            default:
                if (state.GetStateName() != SceneManager.GetActiveScene().name)
                {
                    LoadScene(state);
                }
                else
                {
                    Debug.Log(string.Format("当前已处于{0}场景", this.state.GetStateName()));
                }
                break;
        }
    }

    void LoadScene(IState state, IState sState = null)
    {
        isLoadFinish = false;
        this.state.OnStateEnd();
        SceneManager.LoadScene(state.GetStateName());
        this.state = state;

        if (sState != null)
            GameManager.Instance.StartCoroutine(LoadSceneAsync(sState));
    }

    IEnumerator LoadSceneAsync(IState state)
    {
        yield return null;
        AsyncOperation loadSceneAo = SceneManager.LoadSceneAsync(state.GetStateName());
        loadSceneAo.allowSceneActivation = false;

        while (!loadSceneAo.isDone)
        {
            if (loadSceneAo.progress >= 0.9f)
            {
                //if (Input.GetKeyDown(KeyCode.Space))
                //{
                    GameManager.Instance.uiManager.PopPanel();
                    loadSceneAo.allowSceneActivation = true;
                    this.state = state;
                //}
            }
            yield return null;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //ui重载
        GameManager.Instance.uiManager.GetCurrentSceneUIObject();

        //场景start
        state.OnStateStart();

        //允许场景update
        isLoadFinish = true;
    }
}
