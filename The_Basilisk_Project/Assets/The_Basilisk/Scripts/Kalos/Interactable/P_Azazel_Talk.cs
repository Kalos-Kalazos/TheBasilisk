using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class P_Azazel_Talk : MonoBehaviour
{
    public bool ended, talking, onRange, canNext;

    P_GameManager gm;

    [SerializeField] PlayableDirector tml;

    [SerializeField] GameObject dialoguePanelUI, combatUI;

    [SerializeField] TMP_Text dialogueText;

    [SerializeField, TextArea(4, 6)] string[] dialogueLines;

    public int lineIndex;

    float typingTime = 0.05f;
    float w8Time = 0.75f;

    private void Start()
    {
        gm = FindAnyObjectByType<P_GameManager>();

        talking = false;
        ended = false;
        dialoguePanelUI.SetActive(false);
    }

    private void Update()
    {
        if (talking && !ended)
        {
            if (dialogueText.text == dialogueLines[lineIndex])
            {
                w8Time -= Time.deltaTime;
            }

            if (w8Time <= 0)
            {
                canNext = true;
                w8Time = 0.75f;
            }
            else canNext = false;
        }

        /*
        if (lineIndex > dialogueLines.Length)
        {
            lineIndex = dialogueLines.Length;
        }*/
    }

    public void AzazelTalks()
    {
        if (!talking || ended) return;

        //logica del cambio de camara
        tml.Play();

        //logica del cuadro de texto
        combatUI.SetActive(false);
        dialoguePanelUI.SetActive(true);
        lineIndex = 0;
        StartCoroutine(StartDialogue());

        gm.player.SetActive(false);
    }

    public void NextLine()
    {
        lineIndex++;
        if (lineIndex < dialogueLines.Length)
        {
            StartCoroutine(StartDialogue());
        }
        else
        {
            talking = false;
            ended = true; 
            AzazelOpens();
        }
    }

    private IEnumerator StartDialogue()
    {
        dialogueText.text = string.Empty;

        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSeconds(typingTime);
        }
    }

    public void PauseTimeLine()
    {
        tml.Pause();
    }

    public void AzazelOpens()
    {
        gm.player.SetActive(true);

        //logica del cambio de camara de vuelta
        tml.Play();

        //logica del cuadro de texto de vuelta
        combatUI.SetActive(true);
        dialoguePanelUI.SetActive(false);

        gm.CheckToOpen();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onRange = false;
        }
    }
}
