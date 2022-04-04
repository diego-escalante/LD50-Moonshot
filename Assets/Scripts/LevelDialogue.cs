using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelDialogue : MonoBehaviour {

    public DialogueScript initialScript;
    public DialogueScript successScript;
    public DialogueScript failedScript;
    private DialogueManager manager;
    private RocketStartSequence startSequence;
    private RawImage fader;
    
    private void Awake() {
        manager = GetComponent<DialogueManager>();
        startSequence = GetComponent<RocketStartSequence>();
        fader = GameObject.Find("Center Canvas").transform.Find("RawImage").GetComponent<RawImage>();
    }

    private void OnEnable() {
        EventManager.StartListening(EventManager.Event.Succeeded, Succeeded);
        EventManager.StartListening(EventManager.Event.Failed, Failed);
    }

    private void Start() {
        StartCoroutine(InitialDialogue());
    }

    private IEnumerator InitialDialogue() {
        yield return new WaitForSeconds(3f);
        manager.StartDialogue(initialScript);

        while (true) {
            if (!manager.IsOpen) {
                startSequence.enabled = true;
                yield break;
            }
            yield return null;
        }
    }

    private void Failed() {
        manager.StartDialogue(failedScript);
        StartCoroutine(LoadNextScene());
    }

    private void Succeeded() {
        manager.StartDialogue(successScript);
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene() {
        while (true) {
            if (!manager.IsOpen) {
                // yield return new WaitForSeconds(2f);

                float totalTime = 2.5f;
                for (float elapsedTime = 0; elapsedTime < totalTime; elapsedTime += Time.deltaTime) {
                    fader.color = Color.Lerp(Color.clear, Color.black, elapsedTime / totalTime);
                    yield return null;
                }
                
                SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
                yield break;
            }
            yield return null;
        }
    }

}
