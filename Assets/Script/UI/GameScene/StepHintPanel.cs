using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StepHintPanel : IPanel
{
    Canvas canvas;
    GameObject panel;
    Text txtHint;
    PlayerCtrl playerCtrl;

    public string GetPanelName()
    {
        return "StepHintPanel";
    }

    public void OnEnter()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        panel = canvas.transform.Find("StepHintPanel").gameObject;
        txtHint = panel.transform.Find("TxtHint").GetComponent<Text>();
        playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
        GameManager.Instance.sequenceManager.StepChangeEvent += OnDescUpdate;

        if (playerCtrl.AtTable == GameManager.Instance.sequenceManager.TargetTable)
            txtHint.text = GameManager.Instance.sequenceManager.CurStep.Desc;
        else
            txtHint.text = "你现在应该去另一张桌子";

        if (!GameManager.Instance.sequenceManager.IsCurSeqOver)
            panel.SetActive(true);
    }

    public void OnExit()
    {
        panel.SetActive(false);
        GameManager.Instance.sequenceManager.StepChangeEvent -= OnDescUpdate;
    }

    public void OnPause()
    {

    }

    public void OnUpdate()
    {

    }

    public void OnDescUpdate()
    {
        if (playerCtrl.AtTable == GameManager.Instance.sequenceManager.TargetTable)
            txtHint.text = GameManager.Instance.sequenceManager.CurStep.Desc;
        else
            txtHint.text = "你现在应该去另一张桌子";

        if (GameManager.Instance.sequenceManager.IsCurSeqOver)
            panel.SetActive(false);
    }
}
