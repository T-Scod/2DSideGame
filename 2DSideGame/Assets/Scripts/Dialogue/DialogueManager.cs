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
        m_dialogueBox.SetActive(false);
    }

    //[Tooltip("Drag NPC name text to this slot")]
    //[SerializeField]
    //private TextMeshProUGUI m_nameText;

    [Tooltip("Drag NPC dialogue text to this slot")]
    [SerializeField]

    private TextMeshProUGUI m_dialogueText;
    public Image m_dialoguePortrait;
    public GameObject m_dialogueBox;

    [Tooltip("How fast will the NPC text type")]
    [SerializeField]
    private float m_textSpeed = .008f;

    private bool isCurrentlyTyping; // checks if the dialogue text is typing or if its finished
    private string m_completeText; // this auto completes the text if the user decides to skip dialogue

    [SerializeField]
    private Animator m_anim;

    [SerializeField]
    private Queue<DialogueInformation.Info> m_dialogueInfo = new Queue<DialogueInformation.Info>();

    /// <summary>
    /// Invokes the dialogue box to turn off once it has moved off the screen.
    /// </summary>
    public void InvokeAnim()
    {
        Invoke("TurnOffDialogueBox", 3f);
    }

    /// <summary>
    /// This function loads up the queue with lines of dialogue
    /// </summary>
    /// <param name="info"></param>
    public void EnqueueDialogue(DialogueInformation info)
    {
        m_dialogueBox.SetActive(true);
        m_anim.SetBool("isOpen", true);
        m_dialogueInfo.Clear();

        foreach (DialogueInformation.Info diaInfo in info.m_dialogueInfo)
        {
            m_dialogueInfo.Enqueue(diaInfo);
        }

        DequeueDialogue();
    }

    /// <summary>
    /// Removes and returns the next dialogue in the queue.
    /// </summary>
    public void DequeueDialogue()
    {
        if (isCurrentlyTyping)
        {
            CompleteText();
            StopAllCoroutines();
            isCurrentlyTyping = false;
            return;
        }

        if (m_dialogueInfo.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueInformation.Info info = m_dialogueInfo.Dequeue();
  
        m_completeText = info.m_text;
       // m_nameText.text = info.m_character.m_name;
        m_dialogueText.text = info.m_text;
        m_dialoguePortrait.sprite = info.m_character.m_portrait;

        m_dialogueText.text = "";
        StartCoroutine(TypeSentence(info, m_textSpeed));
    }

    /// <summary>
    /// This ends the conversation and invokes the dialogue box to disappear after a certain amount of time.
    /// </summary>
    public void EndDialogue()
    {
        m_anim.SetBool("isOpen", false);
        InvokeAnim();
    }

    /// <summary>
    /// Sets the dialogue box to false. This function gets called by the invoke function.
    /// </summary>
    public void TurnOffDialogueBox()
    {
        m_dialogueBox.SetActive(false);
    }

    /// <summary>
    /// Animates the dialogue to type out with a delay.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="textSpeed"></param>
    /// <returns></returns>
    IEnumerator TypeSentence(DialogueInformation.Info info, float textSpeed)
    {
        isCurrentlyTyping = true;
        foreach (char letter in info.m_text.ToCharArray()) // converts string to a character array
        {
            m_dialogueText.text += letter; // appends letter to the end of the string
            yield return new WaitForSeconds(textSpeed);
        }
        isCurrentlyTyping = false;
    }

    /// <summary>
    /// Auto completes the text if the player decides he wants to skip the dialogue.
    /// </summary>
    private void CompleteText()
    {
        m_dialogueText.text = m_completeText;
    }
}
