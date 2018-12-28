using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipePanel : IPanel
{
    Canvas canvas;
    GameObject recipePanel;
    GameObject content;
    List<Transform> recipes;
    ScrollRect scrollRect;
    Button cancelButton;
    Button okButton;
    Button laButton;
    Button raButton;

    float targetScrollPosition;
    int currentRecipeIndex = 0;

    public string GetPanelName()
    {
        return "RecipePanel";
    }

    public void OnEnter()
    {
        canvas = GameObject.FindObjectOfType<Canvas>();
        content = canvas.transform.Find("RecipePanel/Scroll View/Viewport/Content").gameObject;
        recipes = new List<Transform>();
        recipePanel = canvas.transform.Find("RecipePanel").gameObject;
        cancelButton = canvas.transform.Find("RecipePanel/CancelButton").GetComponent<Button>();
        okButton = canvas.transform.Find("RecipePanel/OkButton").GetComponent<Button>();
        scrollRect = canvas.transform.Find("RecipePanel/Scroll View").GetComponent<ScrollRect>();
        laButton = canvas.transform.Find("RecipePanel/LeftArrowButton").GetComponent<Button>();
        raButton = canvas.transform.Find("RecipePanel/RightArrowButton").GetComponent<Button>();

        recipePanel.SetActive(true);
        InitRecipeList();
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

    public void OnUpdate()
    {
        Debug.Log(currentRecipeIndex);
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
        Debug.Log(string.Format("你选择了{0}号菜谱", currentRecipeIndex));
    }

    void OnLaButtonClick()
    {
        SetCurrentRescipelIndex(false);
        MoveScrollRect(true);
    }

    void OnRaButtonClick()
    {
        SetCurrentRescipelIndex(true);
        MoveScrollRect(false);
    }

    void InitRecipeList()
    {
        recipes.AddRange(content.transform.GetComponentsInChildren<Transform>(true));
        recipes.RemoveAt(0);
    }

    void SetCurrentRescipelIndex(bool isPlus)
    {
        if (isPlus)
        {
            currentRecipeIndex++;
            currentRecipeIndex = Mathf.Clamp(currentRecipeIndex, 0, recipes.Count - 1);
        }
        else
        {
            currentRecipeIndex--;
            currentRecipeIndex = Mathf.Clamp(currentRecipeIndex, 0, recipes.Count - 1);
        }
    }

    void MoveScrollRect(bool isLeft)
    {
        float targetP;
        if (isLeft)
        {
            targetP = Mathf.Clamp01(currentRecipeIndex* (1f / (recipes.Count-1f)));
            scrollRect.horizontalNormalizedPosition = targetP;
        }
        else
        {
            targetP = Mathf.Clamp01(currentRecipeIndex * (1f / (recipes.Count-1f)));
            scrollRect.horizontalNormalizedPosition = targetP;
        }
    }
}
