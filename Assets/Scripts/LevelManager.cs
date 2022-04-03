using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    
    public float levelDuration = 30f;
    private AsteroidSpawner spawner;
    private Transform moon;
    
    private void Awake() {
        spawner = GetComponent<AsteroidSpawner>();
        moon = GameObject.FindWithTag("Moon").transform;
    }
    
    private void Update() {
        levelDuration -= Time.deltaTime;

        if (levelDuration <= 0) {
            levelDuration = 0;
            spawner.enabled = false;
            enabled = false;
            StartCoroutine(FinalSequence());
        }
    }

    private IEnumerator FinalSequence() {
        while (true) {
            // Wait for all asteroids to despawn.
            if (spawner.AreAllAsteroidsInactive()) {
                break;
            }
            yield return null;
        }

        RocketController controller = GameObject.FindWithTag("Rocket").GetComponent<RocketController>();
        if (!controller.enabled) {
            // Return early... Rocket probably exploded before all the asteroids cleared out.
            yield break;
        }
        
        // Move moon into place.
        while (true) {
            moon.Translate(Vector2.down * 3f * Time.deltaTime);
            if (moon.position.y <= 4.75f) {
                moon.position = new Vector3(moon.position.x, 4.75f, moon.position.z);
                break;
            }
            yield return null;
        }

        controller.OverrideUpInput();

    }
}
