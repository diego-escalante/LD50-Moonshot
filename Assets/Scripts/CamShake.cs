using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour {

    private bool doneMoving = true;
    private bool doneShaking = true;

    private void OnEnable() {
        EventManager.StartListening(EventManager.Event.RocketHit, BigShake);
    }

    private void OnDisable() {
        EventManager.StopListening(EventManager.Event.RocketHit, BigShake);
    }

    public void TinyShake() {
        if (doneShaking) StartCoroutine(shake(10, 0.3f, 0.015f, true, Vector2.zero));
    }

    public void BigShake() {
        if (doneShaking) StartCoroutine(shake(30, 0.75f, 0.015f, true, Vector2.zero));
    }

    private IEnumerator shake(int amount, float range, float duration, bool decay, Vector2 direction) {
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

            StartCoroutine(smoothMove(newPos, duration));
            while (!doneMoving) { yield return null; }
        }

        StartCoroutine(smoothMove(origin, duration));
        while (!doneMoving) { 
            yield return null;
        }
        doneShaking = true;
    }

    private IEnumerator smoothMove(Vector3 targetPosition, float duration) {
        doneMoving = false;
        Vector3 originalPosition = transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            transform.localPosition = new Vector3(Mathf.SmoothStep(originalPosition.x, targetPosition.x, elapsedTime / duration),
                                                  Mathf.SmoothStep(originalPosition.y, targetPosition.y, elapsedTime / duration),
                                                  originalPosition.z);
            yield return null;
        }

        transform.localPosition = new Vector3(targetPosition.x, targetPosition.y, originalPosition.z);
        doneMoving = true;
    }
}