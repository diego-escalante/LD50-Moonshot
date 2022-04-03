using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour {

    //TODO: Enable the general use of rich text
    //TODO: Make quickscroll more accurate
    //TODO: I dislike that the speakers are in an Array and you must use the index.
    // I rather have a map of speaker name keys that you can use to assign a speaker 
    // name to the subdialogue, but Unity doesn't serialize maps for you for free.

    public bool IsOpen {get; set;} = false;
    public float quickScrollSpeed = 0.005f;
    public TMP_Text leftText;
    public TMP_Text rightText;

    private Queue<DialogueScript.SubDialogue> subDialogues = new Queue<DialogueScript.SubDialogue>();
    private const string isOpenStr = "IsOpen";
    private Coroutine co = null;
    private bool isQuickScrolling = false;

    private Animator animator;
    private AudioSource audioSource;

    private void Start() {
        
        // Sound - Optional
        audioSource = GetComponent<AudioSource>();

        // Animation - Optional
        animator = GetComponent<Animator>();
        if (animator != null) {
            animator.SetBool(isOpenStr, IsOpen);
        }
    }

    public void Update() {
        if (IsOpen && Input.GetKeyDown(KeyCode.Space)) {
            if (co == null) {
                DisplayNextText();
            } else {
                isQuickScrolling = true;
            }
        }
    }

    public void StartDialogue(DialogueScript dialogue) {
        // If there's a dialogue already in progress, don't start a new one.
        if (IsOpen) {
            return;
        }
        IsOpen = true;

        subDialogues.Clear();
        foreach (DialogueScript.SubDialogue subDialogue in dialogue.GetSubDialogues()) {
            subDialogues.Enqueue(subDialogue);
        }

        gameObject.SetActive(true);

        if (animator != null) {
            animator.SetBool(isOpenStr, IsOpen);
        }

        DisplayNextText();
    }

    public void DisplayNextText() {
        leftText.transform.parent.gameObject.SetActive(false);
        rightText.transform.parent.gameObject.SetActive(false);
        if (subDialogues.Count == 0) {
            co = StartCoroutine(CloseDialogueBox());
            return;
        }

        DialogueScript.SubDialogue subDialogue = subDialogues.Dequeue();
        co = StartCoroutine(ScrollSubDialogue(subDialogue));
    }

    private IEnumerator ScrollSubDialogue(DialogueScript.SubDialogue subDialogue) {
        DialogueScript.Speaker speaker = subDialogue.Speaker;

        // Set up subdialogue UI
        TMP_Text uiText;
        if (speaker.profileLocation == DialogueScript.ProfileLocation.Left) {
            uiText = leftText;
        } else {
            uiText = rightText;
        }
        uiText.transform.parent.gameObject.SetActive(true);
        uiText.fontSize = speaker.fontSize;
        uiText.color = speaker.fontColor;
        // buttonPrompt.SetActive(false);

        // Set up subdialogue sound
        if (audioSource != null && speaker.scrollSound != null) {
            audioSource.clip = speaker.scrollSound;
        }

        // Animate text scroll
        bool inBrackets = false;
        string text = subDialogue.Text;
        for(int i = 0; i < text.Length; i++) {
            audioSource.pitch = speaker.pitch + Random.Range(-speaker.pitchDelta/2, speaker.pitchDelta/2);
            // Special case: Angular brackets. If an open bracket exists, skip until closing bracket is found.
            // TODO: This only works for rich text with something like <sprite=0>, not something like <i>italics</i>.
            // This is because the <i> will be processed individually and will show in the UI until </i> is also processed
            // individual.
            // TODO: This also doesn't handle other special cases like escaping angular brackets \< \>, nor nested brackets,
            // nor brackets used outside of rich text. (e.g. "I wrote angle brackets like <this> on my notebook".)
            // TODO: Just skip forward instead of using the top-level loop and the inBrackets variable.
            if (text[i] == '<') {
                inBrackets = true;
            } else if (text[i] == '>') {
                inBrackets = false;
            } 
            if (inBrackets) {
                continue;
            }

            // Play a sound if it is desired and the new character is not a space.
            if (text[i] != ' ' && audioSource != null && speaker.scrollSound != null) {
                audioSource.Play();
            }

            string revealedText = text.Substring(0, i+1);
            string hiddenText = "<color=#00000000>" + Regex.Replace(text.Substring(i+1), @"<sprite=\d+>", "<sprite=0>") + "</color>";
            uiText.text = revealedText + hiddenText;
            yield return new WaitForSecondsRealtime(isQuickScrolling ? quickScrollSpeed : speaker.scrollInterval);
        }

        // Done
        // buttonPrompt.SetActive(true);
        isQuickScrolling = false;
        co = null;
    }

    private IEnumerator CloseDialogueBox() {
        if (animator != null) {
            animator.SetBool(isOpenStr, false);
            yield return null;
            while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 || animator.IsInTransition(0)) {
                yield return null;
            }
        }

        // gameObject.SetActive(false);
        leftText.transform.parent.gameObject.SetActive(false);
        rightText.transform.parent.gameObject.SetActive(false);
        IsOpen = false;
    }
}
