using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(CapsuleCollider2D), typeof (SpriteRenderer))]
public class RocketCollisionManager : MonoBehaviour {

    public TMP_Text uiText;
    [SerializeField] private LayerMask asteroidMask, moonMask;
    [SerializeField] private float invincibilityTime = 3f;
    [SerializeField] private float flashFadeTime = 0.25f;
    [SerializeField] private float flashInterval = 0.5f;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private int hits = 2;
    
    private Collider2D coll;
    private SpriteRenderer rend;
    private Collider2D[] others = new Collider2D[5];
    private ContactFilter2D asteroidFilter, moonFilter;
    private bool isInvincible;
    private Coroutine flashCoroutine;
    private Volume vol;

    private void Awake() {
        coll = GetComponent<Collider2D>();
        rend = GetComponent<SpriteRenderer>();
        asteroidFilter.SetLayerMask(asteroidMask);
        moonFilter.SetLayerMask(moonMask);
        uiText.text = "Hull Integrity: " + hits;
    }

    private void Start() {
        vol = GameObject.FindWithTag("Volume").GetComponent<Volume>();
    }

    private void Update() {
        // Check if rocket overlaps moon.
        if (coll.OverlapCollider(moonFilter, others) > 0) {
            StartCoroutine(Explode(true));
        }
        
        // Check if any asteroids overlap the rocket.
        if (!isInvincible && coll.OverlapCollider(asteroidFilter, others) > 0) {
            // TODO: This GetComponent call could be cached, there's only a limited number of asteroids in the pool.
            for (int i = 0; i < others.Length; i++) {
                if (others[i] == null) {
                    continue;
                }

                if (Vector2.Distance(others[i].transform.position, transform.position) < 1.5f) {
                    Hit();
                    others[i].GetComponent<AsteroidBehavior>().Crash();
                    return;
                }
            }
        }
    }

    private void Hit() {
        hits--;
        uiText.text = "Hull Integrity: " + hits;
        if (hits > 0) {
            EventManager.TriggerEvent(EventManager.Event.RocketHit);
            StartCoroutine(Invincible());
        } else {
            StartCoroutine(Explode(false));
        }
    }

    private IEnumerator Explode(bool isSuccess) {
        enabled = false;
        EventManager.TriggerEvent(EventManager.Event.PreExplosion);
        GetComponent<RocketController>().enabled = false;
        isInvincible = true;
        Time.timeScale = 0;
        vol.enabled = true;
        yield return new WaitForSecondsRealtime(0.5f);

        Transform explosion = transform.Find("Explosion");
        EventManager.TriggerEvent(EventManager.Event.RocketExplode);
        for (float timeLeft = 2f; timeLeft > 0; timeLeft -= Time.unscaledDeltaTime) {
            explosion.localScale = Vector3.one * Mathf.Lerp(30, 0, timeLeft / 2);
            yield return null;
        }
        vol.enabled = false;
        Time.timeScale = 1;

        SpriteRenderer explosionRend = explosion.GetComponent<SpriteRenderer>();
        for (float timeLeft = 2f; timeLeft > 0; timeLeft -= Time.deltaTime) {
            explosionRend.color = Color.Lerp(Color.black, Color.white, timeLeft / 2f);
            yield return null;
        }

        EventManager.TriggerEvent(isSuccess ? EventManager.Event.Succeeded : EventManager.Event.Failed);
    }

    private IEnumerator Invincible() {
        // Become invincible.
        isInvincible = true;

        vol.enabled = true;
        // Camera.main.backgroundColor = Color.white;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1;
        // Camera.main.backgroundColor = Color.black;
        vol.enabled = false;
        
        // Flash during invincibility duration.
        flashCoroutine = StartCoroutine(Flash());
        yield return new WaitForSeconds(invincibilityTime);

        // Stop being invincible, clean up.
        if (flashCoroutine != null) {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }
        rend.material.color = Color.clear;
        isInvincible = false;
    }

    private IEnumerator Flash() {
        Color clearColor = new Color(flashColor.r, flashColor.g, flashColor.b, 0);
        float timeLeft = flashFadeTime;
        while (timeLeft > 0) {
            rend.material.SetColor("_Color", Color.Lerp(clearColor, flashColor, timeLeft / flashFadeTime));
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(flashInterval);
        flashCoroutine = StartCoroutine(Flash());
    }
}
