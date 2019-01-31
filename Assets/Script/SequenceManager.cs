using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttentionType
{
    QuitTable,
    KeyDown,
    TimeOver,

}

public class SequenceManager
{
    private Dictionary<string, List<SequenceData>> sequenceDict = new Dictionary<string, List<SequenceData>>();
    public string CurSequence { get; private set; }
    public SequenceData CurStep { get; private set; }
    public AttentionType Attention { get; private set; }
    public object AttentionParam { get; private set; }
    int step = -1;

    public void LoadSequence(string sequenceName)
    {
        List<SequenceData> s = JsonHelper.ReadJson<List<SequenceData>>("Sequences/" + sequenceName);
        sequenceDict.Add(sequenceName, s);
    }

    public void StartSequence(string sequenceName)
    {
        if (!sequenceDict.ContainsKey(sequenceName))
        {
            Debug.LogError("Try to Start an Unload Sequence");
            return;
        }

        CurSequence = sequenceName;

        BeginStep(++step);
    }

    private void BeginStep(int step)
    {
        //Set Step
        CurStep = sequenceDict[CurSequence][step];

        //Find AttentionType and Param
        AttentionParam = CurStep.Param;
        switch (CurStep.AttentionType)
        {
            case "QuitTable":
                Attention = AttentionType.QuitTable;
                AttentionParam = null;
                break;
            case "ClickButton":
                Attention = AttentionType.KeyDown;
                break;
            case "TimeOver":
                Attention = AttentionType.TimeOver;
                break;
            default:
                break;
        }

        Debug.Log("Step " + step + " Begin:\n" + CurStep.Desc);
    }

    public void StepTriggerHandler(AttentionType attention, object param)
    {
        //Debug.Log("Cur: " + attention + " " + param + " " + (Attention == attention) + " " + (param == AttentionParam));
        //Debug.Log("Trigger: " + Attention + " " + AttentionParam);
        if (Attention == attention)
        {
            bool isMatch = false;
            switch (Attention)
            {
                case AttentionType.QuitTable:
                    isMatch = true;
                    break;
                case AttentionType.KeyDown:
                    isMatch = (int)param == (int)AttentionParam;
                    break;
                case AttentionType.TimeOver:
                    break;
                default:
                    break;
            }

            if (isMatch)
            {
                Debug.Log("Step " + step + " Over");
                if (step < sequenceDict[CurSequence].Count - 1)
                    BeginStep(++step);
                else
                    OverSequence();
            }
        }
    }

    private void OverSequence()
    {

    }
}
