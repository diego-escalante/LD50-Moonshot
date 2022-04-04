using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidSpawner : MonoBehaviour {

    [SerializeField] private float maxRotationDegreesPerSecond = 20f;
    [SerializeField] private int poolSize = 20;
    [SerializeField] private List<GameObject> asteroidPrefabs = new List<GameObject>();
    [SerializeField] private Color[] asteroidColors;
    [SerializeField] private float maxHorizontalSpeed = 2f;
    [SerializeField] private float minVerticalSpeed = 1f;
    [SerializeField] private float maxVerticalSpeed = 3f;
    [SerializeField] private float spawnInterval = 1f;
    
    private List<AsteroidBehavior> asteroidPool = new List<AsteroidBehavior>();
    private int currentAsteroid = 0;
    private Camera cam;
    private Coroutine co;
    
    private void Awake() {
        // Create all asteroids up front.
        for (int i = 0; i < poolSize; i++) {
            AsteroidBehavior newAsteroid = Instantiate(asteroidPrefabs[i % asteroidPrefabs.Count]).GetComponent<AsteroidBehavior>();
            newAsteroid.gameObject.SetActive(false);
            asteroidPool.Add(newAsteroid);
        }
    }

    private void OnEnable() {
        co = StartCoroutine(Spawner());
    }
    
    private void OnDisable() {
        StopCoroutine(co);
    }

    private void Start() {
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    private IEnumerator Spawner() {
        while (true) {
            yield return new WaitForSeconds(spawnInterval);
            SpawnAsteroid();
        }
    }

    private void DespawnAllAsteroids() {
        asteroidPool.ForEach(i => i.gameObject.SetActive(false));
        currentAsteroid = 0;
    }

    private void SpawnAsteroid() {
        // Calculate possible spawn locations.
        Vector2 maxViewportPosInWorldSpace = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));
        float verticalSpawnPoint = maxViewportPosInWorldSpace.y + 0.5f;
        float maxHorizontalSpawnPoint = maxViewportPosInWorldSpace.x - 0.5f;

        AsteroidBehavior asteroid = FindNextAvailableAsteroid();
        if (asteroid == null) {
            Debug.LogWarning("Unable to spawn asteroid! All asteroids in pool are currently active.");
            return;
        }
        asteroid.gameObject.SetActive(true);
        asteroid.Initialize(
            new Vector2(Random.Range(-maxHorizontalSpawnPoint, maxHorizontalSpawnPoint), verticalSpawnPoint), 
            new Vector2(Random.Range(-maxHorizontalSpeed, maxHorizontalSpeed), Random.Range(-minVerticalSpeed, -maxVerticalSpeed)), 
            Random.Range(0, 360), 
            Random.Range(-maxRotationDegreesPerSecond, maxRotationDegreesPerSecond), 
            asteroidColors[Random.Range(0, asteroidColors.Length)]);
    }

    /// <summary>
    /// Finds the next asteroid in the pool that is inactive.
    /// </summary>
    /// <returns> Returns an asteroid ready to use, or null if no asteroid is available at the time. </returns>
    private AsteroidBehavior FindNextAvailableAsteroid() {
        AsteroidBehavior asteroid;
        for (int searchCount = poolSize; searchCount > 0; searchCount--) {
            asteroid = asteroidPool[currentAsteroid % poolSize];
            if (!asteroid.gameObject.activeInHierarchy) {
                return asteroid;
            }
            currentAsteroid++;
        }
        return null;
    }

    public bool AreAllAsteroidsInactive() {
        foreach (AsteroidBehavior asteroid in asteroidPool) {
            if (asteroid.isActiveAndEnabled) {
                return false;
            }
        }
        return true;
    }
}
