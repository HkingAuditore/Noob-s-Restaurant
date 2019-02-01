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
        return "SFTableHintPanel";
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
            txtHint.text = "You may be at a wrong table";

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
            txtHint.text = "You may be at a wrong table";
    }
}
