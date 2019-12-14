using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPlayer : MonoBehaviour
{
    public QuestGiver giver;

    // Start is called before the first frame update
    void Start()
    {
        giver = FindObjectOfType<QuestGiver>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            giver.AcceptQuest();
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            QuestManager.OnEnemyKilled(0);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            QuestManager.OnNpcConversation("Michael");
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            QuestManager.OnAreaEntered(0);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.ClearDeveloperConsole();
        }
    }
}
