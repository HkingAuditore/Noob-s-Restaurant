using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ConversationPanel : MonoBehaviour
{
    public string CharacterName;
    public string Dialog;
    public float PauseTime;

    private Image _avatar;
    private Text _name;
    private Text _dialog;

    private string _filePath ="UI/Avatars/";



    private void Awake()
    {
        _avatar = this.GetComponentsInChildren<Image>()[0];
        _name = this.GetComponentsInChildren<Text>()[0];
        _dialog = this.GetComponentsInChildren<Text>()[1];

        RefreshAll(CharacterName,Dialog);

    }

    // 替换立绘
    private void ReplaceAvatar(string name)
    {
        _avatar.sprite = Resources.Load(_filePath + name,typeof(Sprite)) as Sprite;
    }

    //替换姓名
    private void ReplaceName(string name)
    {
        _name.text = name + ":";
    }


    /// <summary>
    /// 全部刷新
    /// </summary>
    /// <param name="chararcterName">角色名</param>
    /// <param name="dialog">角色对白</param>
    public void RefreshAll(string chararcterName, string dialog)
    {
        ReplaceAvatar(chararcterName);
        ReplaceName(chararcterName);
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
        StringBuilder dialogBuilder = new StringBuilder("       ");     

        //打字机效果
        while (curPos < dialog.Length)
        {
            dialogBuilder.Append(dialog[curPos++]);
            showDialog.text = dialogBuilder.ToString();
            yield return wait;
        }
    }
}
