using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Original
{
    public class OriginalLibrary : MonoBehaviour
    {
        public List<OriginalItemBaseClass> items = new List<OriginalItemBaseClass>();

        /// <summary>
        /// test
        /// </summary>
        public void Test()
        {
            if (!OriginalManager.Instance.initDone) return;
            for (int i = 0; i < 10; i++)
            {
                var temp = Add(OriginalType.Tomato);
                //Debug.Log(temp.ID);
            }
        }

        public OriginalItemBaseClass Add(OriginalType type)
        {
            var temp = OriginalCreator.Instance.CreateOriginal(type);
            items.Add(temp);
            return temp;
        }

        public List<OriginalItemBaseClass> Find(OriginalType type)
        {
            return items.FindAll(x => x.data.originalType == type);
        }

        public bool Change() { return false; }

        public bool Delete(int id)
        {
            var t = items.Find(x => x.ID == id);
            OriginalCreator.Instance.DeleteOriginal(t);
            return items.Remove(t);
        }

        public bool Delete(OriginalType type)
        {
            var t = items.Find(x => x.data.originalType == type);
            OriginalCreator.Instance.DeleteOriginal(t);
            return items.Remove(t);
        }

        public bool Delete(OriginalItemBaseClass originalItem)
        {
            OriginalCreator.Instance.DeleteOriginal(originalItem);
            return items.Remove(originalItem);
        }
    }
}