using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Original
{
    public class ClassificationClass : MonoBehaviour
    {
        public Transform tagTransform;
        public GameObject tagPrefab;

        int[] tagNum = new int[64];

        public virtual void OriginalClassification(List<OriginalItemBaseClass> items)
        {

            for (int i = 0; i < items.Count; i++)
            {
                int typeIndex = (int)items[i].data.originalType;
                tagNum[typeIndex]++;
            }
        }
    }
}
