using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndBehavior : MonoBehaviour {
    public RawImage fadeImage;
    
    private void Start() {
        StartCoroutine(Begin());
    }

    private IEnumerator Begin() {
        
        EventManager.TriggerEvent(EventManager.Event.RocketExplode);

        yield return new WaitForSeconds(14f);
        
        float totalTime = 5f;
        for (float timeElapsed = 0; timeElapsed < totalTime; timeElapsed += Time.deltaTime) {
            fadeImage.color = Color.Lerp(Color.black,Color.clear, timeElapsed / totalTime);
            yield return null;
        }
    }
}