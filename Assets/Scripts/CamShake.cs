using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour {

    private bool doneMoving = true;
    private bool doneShaking = true;

    private void OnEnable() {
        EventManager.StartListening(EventManager.Event.RocketHit, BigShake);
        EventManager.StartListening(EventManager.Event.Launching, TinyShake);
        EventManager.StartListening(EventManager.Event.RocketExplode, HugeShake);
    }

    private void OnDisable() {
        EventManager.StopListening(EventManager.Event.RocketHit, BigShake);
        EventManager.StopListening(EventManager.Event.Launching, TinyShake);
        EventManager.StopListening(EventManager.Event.RocketExplode, HugeShake);
    }

    public void TinyShake() {
        if (doneShaking) StartCoroutine(shake(500, 0.15f, 0.015f, true, Vector2.zero, false));
    }

    public void BigShake() {
        if (doneShaking) StartCoroutine(shake(30, 0.85f, 0.015f, true, Vector2.zero, false));
    }
    
    public void HugeShake() {
        if (doneShaking) StartCoroutine(shake(100, 0.75f, 0.015f, false, Vector2.zero, true));
    }

    private IEnumerator shake(int amount, float range, float duration, bool decay, Vector2 direction, bool realTime) {
        doneShaking = false;
        direction = direction.normalized;
        Vector3 origin = transform.localPosition;
        Vector3 newPos = new Vector3();
        float scale = 1;
        int sign = 1;

        for (int i = 0; i < amount; i++) {
            if (decay) scale = (amount - (float)i) / amount;

            if (direction == Vector2.zero) {
                newPos = origin + new Vector3(Random.Range(-range, range) * scale, Random.Range(-range, range) * scale, 0);
            } else {
                newPos = origin + (Vector3)direction * scale * range * sign;
                sign *= -1;
            }

            StartCoroutine(smoothMove(newPos, duration, realTime));
            while (!doneMoving) { yield return null; }
        }

        StartCoroutine(smoothMove(origin, duration, realTime));
        while (!doneMoving) { 
            yield return null;
        }
        doneShaking = true;
    }

    private IEnumerator smoothMove(Vector3 targetPosition, float duration, bool realTime) {
        doneMoving = false;
        Vector3 originalPosition = transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            elapsedTime += realTime ? Time.unscaledDeltaTime : Time.deltaTime;
            transform.localPosition = new Vector3(Mathf.SmoothStep(originalPosition.x, targetPosition.x, elapsedTime / duration),
                                                  Mathf.SmoothStep(originalPosition.y, targetPosition.y, elapsedTime / duration),
                                                  originalPosition.z);
            yield return null;
        }

        transform.localPosition = new Vector3(targetPosition.x, targetPosition.y, originalPosition.z);
        doneMoving = true;
    }
}