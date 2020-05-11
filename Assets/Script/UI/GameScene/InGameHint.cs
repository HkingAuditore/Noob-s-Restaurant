using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameHint : MonoBehaviour
{
    private Image _icon;
    private string _original;

    private string _filePath = "UI/OriginalHintIcon/";

    void Awake()
    {
        _icon = this.GetComponentsInChildren<Image>()[1];
        _original = this.transform.parent.parent.gameObject.GetComponentsInChildren<Ingredient>()[0].gameObject.name;
        _icon.sprite = Resources.Load(_filePath + _original, typeof(Sprite)) as Sprite;
    }


}
