using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipePanel : IPanel
{
    Canvas canvas;
    GameObject recipePanel;
    Button cancelButton;
    Button okButton;
    Button laButton;
    Button raButton;
    ScrollRect scrollRect;
    int recipeCount;
    float targetScrollPosition;
    float speed = 1e+6f;

    public string GetPanelName()
    {
        return "RecipePanel";
    }

    public void OnEnter()
    {
        canvas = GameObject.FindObjectOfType<Canvas>();
        recipePanel = canvas.transform.Find("RecipePanel").gameObject;
        cancelButton = canvas.transform.Find("RecipePanel/CancelButton").GetComponent<Button>();
        okButton = canvas.transform.Find("RecipePanel/OkButton").GetComponent<Button>();
        scrollRect = canvas.transform.Find("RecipePanel/Scroll View").GetComponent<ScrollRect>();
        laButton = canvas.transform.Find("RecipePanel/LeftArrowButton").GetComponent<Button>();
        raButton = canvas.transform.Find("RecipePanel/RightArrowButton").GetComponent<Button>();

        recipePanel.SetActive(true);
        GetRecipeCount();

        cancelButton.onClick.AddListener(OnCancelButtonClick);
        okButton.onClick.AddListener(OnOkButtonClick);
        laButton.onClick.AddListener(OnLaButtonClick);
        raButton.onClick.AddListener(OnRaButtonClick);
    }

    public void OnExit()
    {
        cancelButton.onClick.RemoveListener(OnCancelButtonClick);
        okButton.onClick.RemoveListener(OnOkButtonClick);
        laButton.onClick.RemoveListener(OnLaButtonClick);
        raButton.onClick.RemoveListener(OnRaButtonClick);
        recipePanel.SetActive(false);
    }

    public void OnPause()
    {
        
    }

    void OnCancelButtonClick()
    { 
        GameManager.Instance.uiManager.PopPanel();
    }

    void OnOkButtonClick()
    {
        Debug.Log(string.Format("你选择了{0}号菜谱", GetRecipe()));
    }

    void OnLaButtonClick()
    {
        MoveScrollRect(true);
    }

    void OnRaButtonClick()
    {
        MoveScrollRect(false);
    }

    void GetRecipeCount()
    {
        GameObject content = canvas.transform.Find("RecipePanel/Scroll View/Viewport/Content").gameObject;
        Transform[] recipes = content.GetComponentsInChildren<Transform>();
        recipeCount = recipes.Length-1;
    }

    int GetRecipe()
    {
        float currentHNPosition = scrollRect.horizontalNormalizedPosition;
        int currentRecipeIndex = (int)(currentHNPosition * recipeCount)+1;
        return currentRecipeIndex;
    }

    void MoveScrollRect(bool isLeft)
    {
        Debug.Log("计算");
        if (isLeft)
            targetScrollPosition = ((GetRecipe()-1f) - 1f)/recipeCount;
        else
            targetScrollPosition = ((GetRecipe()-1f) + 1f) / recipeCount;
    }

    void LerpMove(float targetPosition)
    {
        if (scrollRect.horizontalNormalizedPosition != targetPosition)
        {
            scrollRect.horizontalNormalizedPosition = targetPosition;//Mathf.Lerp(scrollRect.horizontalNormalizedPosition, targetPosition, Time.deltaTime * speed);
            Debug.Log("Moving");
        }
    }

    public void OnUpdate()
    {
        Debug.Log("当前：" + scrollRect.horizontalNormalizedPosition);
        Debug.Log("目标：" + targetScrollPosition);
        LerpMove(targetScrollPosition);
    }
}
