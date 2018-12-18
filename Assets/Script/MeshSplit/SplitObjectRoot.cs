using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace SplitMesh
{
    public class SplitObjectRoot : MonoBehaviour
    {
        public SplitInfo[] splitInfos;
        private int splitIndex = 0;
        public List<SplitObject> objectList;
        void Start()
        {
            if (objectList.Count == 0)
            {
                SplitObject rootObject = GetComponentInChildren<SplitObject>();
                objectList.Add(rootObject);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (splitIndex <= splitInfos.Length - 1)
                {
                    SplitInfo splitInfo = splitInfos[splitIndex];
                    List<SplitObject> tempList = new List<SplitObject>();
                    for (int i = 0; i < objectList.Count; i++)
                    {
                        transform.rotation = transform.rotation * Quaternion.Euler(splitInfo.euler);
                        SplitObject splitObject = objectList[i];
                        SplitObject[] childs = splitObject.Split(Vector3.right, splitInfo.distance);
                        if (childs == null)
                            continue;
                        SplitObject child0 = childs[0];
                        SplitObject child1 = childs[1];
                        child0.transform.parent = transform;
                        child1.transform.parent = transform;
                        //TODO:质量的有问题
                        child0.rigidbody.mass = splitObject.rigidbody.mass / 2f;
                        child1.rigidbody.mass = splitObject.rigidbody.mass / 2f;
                        child0.name = splitObject.name + "_0";
                        child1.name = splitObject.name + "_1";
                        objectList.RemoveAt(i);
                        tempList.AddRange(childs);
                        Destroy(splitObject.gameObject);
                        if (i == objectList.Count)
                        {
                            break;
                        }
                        else
                        {
                            i--;
                        }
                    }
                    objectList.AddRange(tempList);
                    splitIndex++;
                }
                else
                {
                    Debug.Log("Finished");
                }
            }
        }
    }
    [Serializable]
    public class SplitInfo
    {
        //每次旋转euler角度，再向右偏移distance距离去切
        public Vector3 euler;
        public float distance;
    }
}