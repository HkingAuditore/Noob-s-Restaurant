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
        if (state != null && isLoadFinish)
        {
            state.OnStateUpdate();
        }
    }

    public void SetState(IState state)
    {
        isLoadFinish = false;

        if (this.state != null)
            this.state.OnStateEnd();

        switch (state.GetStateName())
        {
            //2个特殊场景单独处理
            case "Init":
                Debug.LogWarning("Init场景用于创建gamemanager和初始化,不允许被二次加载");
                return;

            case "Load":
                Debug.LogWarning("load场景不允许被单独加载");
                return;

            default:
                if (state.GetStateName() != this.state.GetStateName())
                {
                    if (state.IsNeedLoadScene())
                    {
                        this.state = new LoadScene();
                        SceneManager.LoadScene(this.state.GetStateName(), LoadSceneMode.Single);
                        GameManager.Instance.StartCoroutine(LoadSceneAsync(state));
                    }
                    else
                    {
                        this.state = state;
                        SceneManager.LoadScene(this.state.GetStateName(), LoadSceneMode.Single);
                    }
                }
                else
                {
                    Debug.Log(string.Format("当前已处于{0}场景", this.state.GetStateName()));
                }
                break;
        }
    }

    IEnumerator LoadSceneAsync(IState state)
    {
        yield return null;
        AsyncOperation loadSceneAo = SceneManager.LoadSceneAsync(state.GetStateName(),LoadSceneMode.Single);
        loadSceneAo.allowSceneActivation = false;

        while (!loadSceneAo.isDone)
        {
            if (loadSceneAo.progress >= 0.9f)
            {
                //if (Input.GetKeyDown(KeyCode.Space))
                //{
                    isLoadFinish = false;
                    GameManager.Instance.uiManager.PopPanel();
                    this.state.OnStateEnd();
                    this.state = state;
                    loadSceneAo.allowSceneActivation = true;

                    break;
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
