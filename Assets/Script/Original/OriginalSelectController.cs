using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Original
{
    public class OriginalSelectController : MonoBehaviour
    {
        public delegate void SelectDoneDelegate(OriginalItemBaseClass[] target);
        public event SelectDoneDelegate SelectDoneEventHandler;

        private void Start()
        {
            SelectDoneEventHandler += delegate { };
        }

        public OriginalLibrary Library;
        public GameObject ItemPrefab;
        public GameObject selectedItemPrefab;
        public Transform itemPanelContent;
        public Transform selectedItemPanelContent;

        public bool IsSelectOneTypeOriginal = true;

        public List<OriginalSelectPanelItems> PanelItemsList = new List<OriginalSelectPanelItems>();
        public List<OriginalSelectedPanelItems> selectedPanelItems = new List<OriginalSelectedPanelItems>();

        int[] _selectedTable = new int[0];

        public void OpenSelectPanel()
        {
            this.gameObject.SetActive(true);
            Debug.Log(Library.items.Count);
            for (int i = 0; i < Library.items.Count; i++)
            {
                //通过枚举类型建立选择表
                var item = Library.items[i];
                if (_selectedTable.Length < (int)item.data.originalType + 1)
                    _selectedTable = new int[(int)item.data.originalType + 1];

                AddToItemPanel(item.data.originalType);
            }
        }

        public void AddToSelected(OriginalType type)
        {
            if (IsSelectOneTypeOriginal)
            {
                for(int i = 0;i< _selectedTable.Length; i++)
                {
                    if (i == (int)type) continue;
                    Debug.Log(i + "/" + _selectedTable[i]);
                    for (int j = _selectedTable[i]; j > 0; j--)
                    {
                        Debug.Log("subcount");
                        SubFromSelected((OriginalType)i);
                    }
                }
            }

            _selectedTable[(int)type]++;

            AddToSelectedPanel(type);
            
            //待选栏中对应项减少
            var item = PanelItemsList.Find(x => x.originalType == type);
            item.AddToSelected();
        }

        public void SubFromSelected(OriginalType type)
        {
            _selectedTable[(int)type]--;

            AddToItemPanel(type);

            //已选栏中对应项减少
            var select = selectedPanelItems.Find(x => x.originalType == type);
            select.Sub();
        }

        /// <summary>
        /// 在待选栏中通过食材类型生成对应食材项
        /// </summary>
        /// <param name="type"></param>
        void AddToItemPanel(OriginalType type)
        {
            var tempPanelItems = PanelItemsList.Find(x => x.originalType == type);
            if (tempPanelItems != null)
            {
                tempPanelItems.Add();
            }
            else
            {
                OriginalSelectPanelItems go = Instantiate(ItemPrefab, itemPanelContent).GetComponent<OriginalSelectPanelItems>();
                go.Init(this, type);
                PanelItemsList.Add(go);
            }
        }

        /// <summary>
        /// 在已选栏中通过食材类型生成对应食材项
        /// </summary>
        /// <param name="type"></param>
        void AddToSelectedPanel(OriginalType type)
        {
            var selected = selectedPanelItems.Find(x => x.originalType == type);
            if (selected != null)
            {
                selected.Add();
            }
            else
            {
                OriginalSelectedPanelItems go = Instantiate(selectedItemPrefab, selectedItemPanelContent).GetComponent<OriginalSelectedPanelItems>();
                go.Init(this, type);
                selectedPanelItems.Add(go);
            }
        }

        /// <summary>
        /// 图集加载预留接口
        /// </summary>
        public void LoadPicture()
        {

        }

        public void SelectDone()
        {
            List<OriginalItemBaseClass> selectedItems = new List<OriginalItemBaseClass>();
            for (int j = 0; j < _selectedTable.Length; j++)
            {
                if (_selectedTable[j] == 0) continue;
                for (int i = 0; i < Library.items.Count; i++)
                {
                    var item = Library.items[i];
                    if ((int)item.data.originalType == j && _selectedTable[j] > 0)
                    {
                        _selectedTable[j]--;
                        selectedItems.Add(item);
                        Debug.Log(item.data.originalType);
                    }
                }
            }

            var temp = selectedItems.ToArray();
            Library.TakeOut(temp);
            SelectDoneEventHandler(temp);
            Quit();
        }

        public void Quit()
        {
            for (int i = PanelItemsList.Count - 1; i >= 0; i--)
            {
                Destroy(PanelItemsList[i].gameObject);
            }
            for (int i = selectedPanelItems.Count - 1; i >= 0; i--)
            {
                Destroy(selectedPanelItems[i].gameObject);
            }
            _selectedTable = new int[0];
            PanelItemsList.Clear();
            selectedPanelItems.Clear();
            this.gameObject.SetActive(false);
        }
    }
}