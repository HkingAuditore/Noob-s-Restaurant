using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStateManager
{
    IState state;
    AsyncOperation loadSceneAo = null;
    GameManager gameManager;
    bool isLoading = false;
    bool isCurrentStartFuncExecuted;

    public SceneStateManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
        isCurrentStartFuncExecuted = true;
        state = new InitScene(this);
        state.OnStateStart();
    }

    public void UpdateState()
    {
        if (!isLoading&& isCurrentStartFuncExecuted)
        {
            state.OnStateUpdate();
        }
        else if (!isLoading && !isCurrentStartFuncExecuted)
        {
            state.OnStateStart();
            isCurrentStartFuncExecuted = true;
        }
    }

    public void SetState(IState state)
    {
        isLoading = true;
        isCurrentStartFuncExecuted = false;
        switch (state.GetStateName())
        {
            //2个特殊场景（init,load）单独处理
            case "Init":
                Debug.LogWarning("Init场景用于创建gamemanager和初始化,不允许被二次加载");
                isLoading = false;
                isCurrentStartFuncExecuted = true;
                return;

            case "Load":
                Debug.LogWarning("load场景不允许被单独加载");
                isLoading = false;
                isCurrentStartFuncExecuted = true;
                return;

            case "Start":
                    LoadScene(state);
                break;

            case "Menu":
                    LoadScene(state);
                break;

            default:
                if (state.GetStateName() != SceneManager.GetActiveScene().name)
                {
                    this.state.OnStateEnd();
                    gameManager.StartCoroutine(LoadSceneAsync(state.GetStateName())); //开启异步加载目标场景
                }
                else
                {
                    Debug.Log(string.Format("当前已处于{0}场景", this.state.GetStateName()));
                    isLoading = false;
                    isCurrentStartFuncExecuted = true;
                }
                break;
        }
    }

    void LoadScene(IState state)
    {
        if (state.GetStateName() != SceneManager.GetActiveScene().name)
        {
            this.state.OnStateEnd();
            SceneManager.LoadScene(state.GetStateName());
            this.state = state;
            isLoading = false;
            isCurrentStartFuncExecuted = false;
        }
        else
        {
            Debug.Log(string.Format( "当前已处于{0}场景中", state.GetStateName()));
            isLoading = false;
            isCurrentStartFuncExecuted = true;
        }
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        //进入加载场景    
        SceneManager.LoadScene("Load");
        loadSceneAo = SceneManager.LoadSceneAsync(sceneName as string);
        loadSceneAo.allowSceneActivation = false;

        while (loadSceneAo.progress < 0.9f)
        {
            yield return new WaitForSeconds(1);
        }
        Debug.Log("load scene: wait 3s");
        yield return new WaitForSeconds(3);        
        while (!Input.anyKeyDown)
        {
            yield return null;
        }
        loadSceneAo.allowSceneActivation = true;
        Debug.Log("load scene: done");
    }
}
