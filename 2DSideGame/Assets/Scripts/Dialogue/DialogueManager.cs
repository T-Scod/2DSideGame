using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Tooltip("Drag NPC name text to this slot")]
    [SerializeField]
    private TextMeshProUGUI m_nameText;
    [Tooltip("Drag NPC dialogue text to this slot")]
    [SerializeField]
    private TextMeshProUGUI m_dialogueText;
    [Tooltip("How fast will the NPC text type")]
    [SerializeField]
    private float m_textSpeed = .008f;

    [SerializeField]
    private Queue<string> m_sentences;

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
        m_sentences = new Queue<string>();
        
    }

    public void StartDialogue(Dialogue dialogue)
    {
        m_nameText.text = dialogue.m_name;

        // Clears previous dialogue on screen
        m_sentences.Clear();

        foreach (string sentence in dialogue.m_sentences)
        {
            // adds sentence to the queue
            m_sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (m_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = m_sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence, m_textSpeed));
    }

    public void EndDialogue()
    {
        Debug.Log("Convo ended");
    }

    IEnumerator TypeSentence(string sentence, float textSpeed)
    {
        m_dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray()) // converts string to a character array
        {
            m_dialogueText.text += letter; // appends letter to the end of the string
            yield return new WaitForSeconds(textSpeed);
        }
    }
}
