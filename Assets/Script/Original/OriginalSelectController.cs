using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Original
{
    public class OriginalSelectController : MonoBehaviour
    {
        public delegate void SelectDoneDelegate(OriginalItemBaseClass[] items);
        public event SelectDoneDelegate SelectDoneEvent;

        private void Start()
        {
            SelectDoneEvent += delegate { };
        }

        public OriginalLibrary library;
        public GameObject itemPrefab;
        public GameObject selectedItemPrefab;
        public Transform itemPanelContent;
        public Transform selectedItemPanelContent;

        public List<OriginalSelectPanelItems> panelItems = new List<OriginalSelectPanelItems>();
        public List<OriginalSelectedPanelItems> selectedPanelItems = new List<OriginalSelectedPanelItems>();

        int[] selectedTable = new int[0];

        public void OpenSelectPanel()
        {
            this.gameObject.SetActive(true);
            Debug.Log(library.items.Count);
            for (int i = 0; i < library.items.Count; i++)
            {
                //通过枚举类型建立选择表
                var item = library.items[i];
                if (selectedTable.Length < (int)item.data.originalType + 1)
                    selectedTable = new int[(int)item.data.originalType + 1];

                AddToItemPanel(item.data.originalType);
            }
        }

        public void AddToSelected(OriginalType type)
        {
            selectedTable[(int)type]++;

            AddToSelectedPanel(type);
            
            //待选栏中对应项减少
            var item = panelItems.Find(x => x.originalType == type);
            item.AddToSelected();
        }

        public void SubFromSelected(OriginalType type)
        {
            selectedTable[(int)type]--;

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
            var tempPanelItems = panelItems.Find(x => x.originalType == type);
            if (tempPanelItems != null)
            {
                tempPanelItems.Add();
            }
            else
            {
                OriginalSelectPanelItems go = Instantiate(itemPrefab, itemPanelContent).GetComponent<OriginalSelectPanelItems>();
                go.Init(this, type);
                panelItems.Add(go);
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
            for (int j = 0; j < selectedTable.Length; j++)
            {
                if (selectedTable[j] == 0) continue;
                for (int i = 0; i < library.items.Count; i++)
                {
                    var item = library.items[i];
                    if ((int)item.data.originalType == j && selectedTable[j] > 0)
                    {
                        selectedTable[j]--;
                        selectedItems.Add(item);
                        Debug.Log(item.data.originalType);
                    }
                }
            }
            var temp = selectedItems.ToArray();
            library.TakeOut(temp);
            SelectDoneEvent(temp);
            Quit();
        }

        public void Quit()
        {
            for (int i = panelItems.Count - 1; i >= 0; i--)
            {
                Destroy(panelItems[i].gameObject);
            }
            for (int i = selectedPanelItems.Count - 1; i >= 0; i--)
            {
                Destroy(selectedPanelItems[i].gameObject);
            }
            selectedTable = new int[0];
            panelItems.Clear();
            selectedPanelItems.Clear();
            this.gameObject.SetActive(false);
        }
    }
}