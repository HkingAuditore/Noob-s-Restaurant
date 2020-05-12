using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationController : MonoBehaviour
{
    [SerializeField]
    List<ConversationNode> conversationNodes;

    int currentNode = 0;

    public void Init()
    {
        conversationNodes[currentNode].gameObject.SetActive(true);
    }

    public void Next()
    {
        if (currentNode == conversationNodes.Count - 1) Quit();
        conversationNodes[currentNode].gameObject.SetActive(false);
        currentNode++;
        conversationNodes[currentNode].gameObject.SetActive(true);
    }

    public void Quit()
    {

    }
}
