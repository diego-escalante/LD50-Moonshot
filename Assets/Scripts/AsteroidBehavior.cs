using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(SpriteRenderer))]
public class AsteroidBehavior : MonoBehaviour {

    private Vector2 vel;
    private Vector2 pos;
    private SpriteRenderer rend;
    private float rotationDegreesPerSecond;
    private static Camera cam;

    private void Awake() {
        rend = GetComponent<SpriteRenderer>();
        if (cam == null) {
            cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        }
    }

    public void Initialize(Vector2 position, Vector2 velocity, float initialRotationDegrees, float rotationDegreesPerSecond, Color color) {
        transform.position = position;
        vel = velocity;
        rend.color = color;
        transform.localEulerAngles = new Vector3(0, 0, initialRotationDegrees);
        this.rotationDegreesPerSecond = rotationDegreesPerSecond;
    }

    private void Update() {
        // Update rotation.
        transform.Rotate(Vector3.forward * rotationDegreesPerSecond * Time.deltaTime);

        // Update position.
        pos = transform.position;
        pos += vel * Time.deltaTime;
        transform.position = pos;
        
        // Deactivate if not needed.
        if (IsOutOfPlayArea()) {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Determines if the asteroid is outside of the play area; that is, if it is under the screen or to the left or
    /// right of it. Asteroids can be above the screen and still be in the play area, as they spawn and move in from
    /// there.
    /// </summary>
    /// <returns>True if the asteroid out of view under, left, or right of the screen.</returns>
    private bool IsOutOfPlayArea() {
        pos = transform.position;
        pos.x = Mathf.Abs(pos.x) - 0.5f;
        pos.y += 0.5f;
        Vector3 vp = cam.WorldToViewportPoint(pos);
        return vp.y < 0 || vp.x > 1;
    }
}
