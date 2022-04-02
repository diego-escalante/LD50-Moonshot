using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class RocketCollisionManager : MonoBehaviour {

    [SerializeField] private LayerMask asteroidMask;
    [SerializeField] private float invincibilityTime = 3f;
    
    private Collider2D coll;
    private Collider2D[] others = new Collider2D[5];
    private ContactFilter2D filter;
    private bool isInvincible;

    private void Awake() {
        coll = GetComponent<Collider2D>();
        filter.SetLayerMask(asteroidMask);
    }

    private void Update() {
        // Check if any asteroids overlap the rocket.
        if (!isInvincible && coll.OverlapCollider(filter, others) > 0) {
            Hit();
        }
    }

    private void Hit() {
        Debug.Log("Hit!");
        EventManager.TriggerEvent(EventManager.Event.RocketHit);
        StartCoroutine(Invincible());
    }

    private IEnumerator Invincible() {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }
}
