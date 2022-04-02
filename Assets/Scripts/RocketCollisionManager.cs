using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D), typeof (SpriteRenderer))]
public class RocketCollisionManager : MonoBehaviour {

    [SerializeField] private LayerMask asteroidMask;
    [SerializeField] private float invincibilityTime = 3f;
    [SerializeField] private float flashFadeTime = 0.25f;
    [SerializeField] private float flashInterval = 0.5f;
    [SerializeField] private Color flashColor = Color.white;
    
    private Collider2D coll;
    private SpriteRenderer rend;
    private Collider2D[] others = new Collider2D[5];
    private ContactFilter2D filter;
    private bool isInvincible;
    private Coroutine flashCoroutine;

    private void Awake() {
        coll = GetComponent<Collider2D>();
        rend = GetComponent<SpriteRenderer>();
        filter.SetLayerMask(asteroidMask);
    }

    private void Update() {
        // Check if any asteroids overlap the rocket.
        if (!isInvincible && coll.OverlapCollider(filter, others) > 0) {
            // TODO: This GetComponent call could be cached, there's only a limited number of asteroids in the pool.
            others[0].GetComponent<AsteroidBehavior>().Crash();
            Hit();
        }
    }

    private void Hit() {
        EventManager.TriggerEvent(EventManager.Event.RocketHit);
        StartCoroutine(Invincible());
    }

    private IEnumerator Invincible() {
        // Become invincible.
        isInvincible = true;
        
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
