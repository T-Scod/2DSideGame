using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager ms_instance;
    public static DialogueManager Instance { get => ms_instance; }

    // Start is called before the first frame update
    void Awake()
    {
        if (ms_instance == null)
        {
            ms_instance = this;
        }
        else
        {
            // If there is another instance destroy it.
            Destroy(gameObject);
            return;
        }

        // Doesnt destroy object this script is connected too when changing between scenes.
        DontDestroyOnLoad(gameObject);    
    }

    [Tooltip("Drag NPC name text to this slot")]
    [SerializeField]
    private TextMeshProUGUI m_nameText;

    [Tooltip("Drag NPC dialogue text to this slot")]
    [SerializeField]

    private TextMeshProUGUI m_dialogueText;
    public Image m_dialoguePortrait;
    public GameObject m_dialogueBox;

    [Tooltip("How fast will the NPC text type")]
    [SerializeField]
    private float m_textSpeed = .008f;

    [SerializeField]
    private Queue<DialogueInformation.Info> m_dialogueInfo = new Queue<DialogueInformation.Info>();



    public void EnqueueDialogue(DialogueInformation info)
    {
        m_dialogueBox.SetActive(true);
        m_dialogueInfo.Clear();

        foreach (DialogueInformation.Info diaInfo in info.m_dialogueInfo)
        {
            m_dialogueInfo.Enqueue(diaInfo);
        }

        DequeueDialogue();
    }

    public void DequeueDialogue()
    {
        if(m_dialogueInfo.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueInformation.Info info = m_dialogueInfo.Dequeue();
        m_nameText.text = info.m_name;
        m_dialogueText.text = info.m_text;
        m_dialoguePortrait.sprite = info.m_portrait;

        StartCoroutine(TypeSentence(info, m_textSpeed));
    }

    public void EndDialogue()
    {
        m_dialogueBox.SetActive(false);
    }


    IEnumerator TypeSentence(DialogueInformation.Info info, float textSpeed)
    {
        m_dialogueText.text = "";
        foreach (char letter in info.m_text.ToCharArray()) // converts string to a character array
        {
            m_dialogueText.text += letter; // appends letter to the end of the string
            yield return new WaitForSeconds(textSpeed);
        }
    }
}
