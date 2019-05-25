using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WorkTask;

public class ProcessDoneNode : WorkflowNode {

    public GameObject ProcessDoneUICanvas;
    public ProcessOriginalTaskMgr taskMgr;
    public GameObject Prompty;
    public Animator CaiXunKunAnimator;

    public override void Init()
    {
        IsNodeDone = false;
        this.gameObject.SetActive(true);
        ProcessDoneUICanvas.SetActive(true);
        CaiXunKunAnimator.gameObject.SetActive(true);
        Prompty.SetActive(false);
        StartCoroutine(WaitForCaiXunKun());
    }

    public override void Quit()
    {
        CaiXunKunAnimator.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        ProcessDoneUICanvas.SetActive(false);
        CaiXunKunAnimator.gameObject.SetActive(false);
        Prompty.SetActive(true);
    }

    private IEnumerator WaitForCaiXunKun()
    {
        yield return new WaitForSeconds(5);
        CaiXunKunAnimator.gameObject.SetActive(false);
        Prompty.SetActive(true);
        IsNodeDone = true;
    }

    //public void PlayAnimation()
    //{
    //    if (Caixukun == null) Caixukun = GetComponent<Image>();
    //    StartCoroutine(PlayAnimationForwardIEnum());
    //}

    //private IEnumerator PlayAnimationForwardIEnum()
    //{
    //    int index = 0;//可以用来控制起始播放的动画帧索引
    //    gameObject.SetActive(true);
    //    while (index > AnimationAmount - 1)
    //    {
    //        //当我们需要在整个动画播放完之后  重复播放后面的部分 就可以展现我们纯代码播放的自由性
    //        //if (index > AnimationAmount - 1)
    //        //{
    //        //    index = 50;
    //        //}
    //        Caixukun.sprite = AnimationSprites[index];
    //        index++;
    //        yield return new WaitForSeconds(0.03f);//等待间隔  控制动画播放速度
    //    }

    //    Caixukun.gameObject.SetActive(false);
    //    Prompty.SetActive(true);
    //}
}
