using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleBehavior : MonoBehaviour {
    public RawImage fadeImage;
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(Begin());
            enabled = false;
        }
    }

    private IEnumerator Begin() {
        float totalTime = 2.5f;
        for (float timeElapsed = 0; timeElapsed < totalTime; timeElapsed += Time.deltaTime) {
            fadeImage.color = Color.Lerp(Color.clear, Color.black, timeElapsed / totalTime);
            yield return null;
        }
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }
}
