using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MessageUIHandler : MonoBehaviour
{
    public TextMeshProUGUI[] textMeshProGUIs;

    Queue messageQueue = new Queue();
    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnGameMessageRecieved(string message)
    {
        Debug.Log($"InGameMessageUIHandler {message}");

        messageQueue.Enqueue(message);

        if (messageQueue.Count > 3)
            messageQueue.Dequeue();

        int queueIndex = 0;

        foreach (string messageInQueue in messageQueue)
        {
            textMeshProGUIs[queueIndex].text = messageInQueue;
            queueIndex++;
        }
    }
}
