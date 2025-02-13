using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class P_Azazel_Talk : MonoBehaviour
{
    public bool ended, talking, onRange, canNext;

    P_GameManager gm;

    [SerializeField] P_CTR_Control chromaticController;
    [SerializeField] PlayableDirector tml;
    [SerializeField] GameObject dialoguePanelUI, combatUI, interactImage;
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
    }

    public void AzazelTalks()
    {
        if (!talking || ended) return;

        // Lógica del cambio de cámara
        tml.Play();

        // Lógica del cuadro de texto
        combatUI.SetActive(false);
        dialoguePanelUI.SetActive(true);
        lineIndex = 0;

        chromaticController.StopOscillation();
        chromaticController.SetAberration(0.15f);

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

        // Lógica del cambio de cámara de vuelta
        tml.Play();

        // Lógica del cuadro de texto de vuelta
        combatUI.SetActive(true);
        dialoguePanelUI.SetActive(false);

        chromaticController.StopOscillation();
        chromaticController.SetAberration(0.6f);
        chromaticController.StartOscillation(); // Reanudar oscilación

        gm.CheckToOpen();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onRange = true;
            if (!ended)
            {
                interactImage.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onRange = false;
            interactImage.SetActive(false);
        }
    }
}