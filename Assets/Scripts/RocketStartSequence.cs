using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketStartSequence : MonoBehaviour {

    [SerializeField] private ParticleSystem[] stars;
    [SerializeField] private Transform planet;
    private AsteroidSpawner asteroidSpawner;
    private RocketController rocketController;

    private void Awake() {
        stars[0].Pause();
        stars[1].Pause();
        asteroidSpawner = GetComponent<AsteroidSpawner>();
        rocketController = GameObject.FindWithTag("Rocket").GetComponent<RocketController>();
    }

    private void OnEnable() {
        StartCoroutine(StartSequence());
    }

    private IEnumerator StartSequence() {
        // Camera shake
        EventManager.TriggerEvent(EventManager.Event.Launching);
        yield return new WaitForSeconds(3f);

        stars[0].Play();
        stars[1].Play();
        float timeLeft = 3f;
        float vel = 0f;
        while (timeLeft > 0) {
            timeLeft -= Time.deltaTime;
            vel += 1.2f * Time.deltaTime;
            planet.Translate(Vector2.down * vel * Time.deltaTime);
            yield return null;
        }

        rocketController.enabled = true;
        
        // Hmm, isn't the moon supposed to be the one doing this?
        Destroy(planet.gameObject);

        yield return new WaitForSeconds(3f);
        asteroidSpawner.enabled = true;
        GetComponent<LevelManager>().enabled = true;
    }

}
