using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LetterPanel : MonoBehaviour
{
    [TextArea(3, 10)] [SerializeField] public string Dialog;
    public float PauseTime;

    private Text _dialog;

    private string _filePath ="UI/Avatars/";



    private void Awake()
    {
        _dialog = this.GetComponentsInChildren<Text>()[0];

        RefreshAll(Dialog);

    }



    /// <summary>
    /// 全部刷新
    /// </summary>
    /// <param name="dialog">角色对白</param>
    public void RefreshAll(string dialog)
    {
        RefreshDialog(dialog);
    }

    /// <summary>
    /// 刷新对白，采用打字机显示
    /// </summary>
    /// <param name="dialog"></param>
    public void RefreshDialog(string dialog)
    {
        StartCoroutine(ShowDialog(PauseTime, dialog, _dialog));
    }

    // 打字机显示对白
    private IEnumerator ShowDialog(float pauseTime,string dialog,Text showDialog)
    {
        WaitForSeconds wait = new WaitForSeconds(pauseTime);
        int curPos = 0;
        //段落前空格
        StringBuilder dialogBuilder = new StringBuilder();     

        //打字机效果
        while (curPos < dialog.Length)
        {
            dialogBuilder.Append(dialog[curPos++]);
            showDialog.text = dialogBuilder.ToString();
            yield return wait;
        }
    }

}
