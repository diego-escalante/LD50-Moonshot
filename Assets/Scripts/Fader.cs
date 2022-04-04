using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour {

    private RawImage img;
    
    private void Start() {
        img = GetComponent<RawImage>();
        StartCoroutine(Fade());
    }
    
    private IEnumerator Fade() {
        float totalTime = 2.5f;
        for (float timeElapsed = 0; timeElapsed <= totalTime; timeElapsed += Time.deltaTime) {
            img.color = Color.Lerp(Color.black, Color.clear, timeElapsed / totalTime);
            yield return null;
        }

        img.color = Color.clear;
    }
    
}
