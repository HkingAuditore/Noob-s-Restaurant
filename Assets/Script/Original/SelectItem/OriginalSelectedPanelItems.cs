using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Original
{
    public class OriginalSelectedPanelItems : MonoBehaviour
    {
        public OriginalType originalType;
        public Text nameText;
        public Text numText;
        public int num;
        OriginalSelectController controller;

        public void Init(OriginalSelectController controller, OriginalType originalType)
        {
            this.originalType = originalType;
            this.controller = controller;
            num = 1;
            nameText.text = originalType.ToString();
            numText.text = num.ToString();
        }

        public void Add()
        {
            num++;
            numText.text = num.ToString();
        }

        public void Sub()
        {
            num--;
            numText.text = num.ToString();
            if (num == 0)
            {
                controller.selectedPanelItems.Remove(this);
                Destroy(this.gameObject);
            }
        }

        public void SubEvent()
        {
            controller.SubFromSelected(originalType);
        }
    }
}