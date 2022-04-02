using System;
using UnityEngine;

public class RocketController : MonoBehaviour {

    [SerializeField] private float speed = 3f;
    [SerializeField] private float timeToTopSpeed = 2f;
    [SerializeField] private string hAxis;
    [SerializeField] private float maxTiltDegrees = 15;

    private float vel;
    private float acc;
    private float targetVel;
    private float currentAcc;
    private float newAcc;
    private float sign;
    private Camera cam;

    private void Awake() {
        acc = speed / timeToTopSpeed;
    }

    private void Start() {
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update() {
        UpdateRocketMovement();
        UpdateRocketTilt();
    }

    /*
    TODO: Possible improvements to tilt:
     1. Only update tilt when velocity changes.
     2. Make tilt take into account max speed. (Tilt more dramatically the faster you move horizontally.)
    */
    private void UpdateRocketTilt() {
        float percentage = (speed - vel) / (speed * 2);
        transform.eulerAngles = new Vector3(0, 0, percentage * maxTiltDegrees * 2 - maxTiltDegrees);
    }

    private void UpdateRocketMovement() {
        // Velocity Verlet: Update position based on x += vt + 0.5*at^2
        Vector3 pos = transform.position;
        pos.x += vel * Time.deltaTime + 0.5f * currentAcc * Time.deltaTime * Time.deltaTime;
        
        // Don't let the rocket go out of camera view.
        // (TODO: This is brittle because it depends on the camera being centered at x = 0.)
        // (TODO: This doesn't need to be calculated every frame.)
        float maxHorizontalDistance = cam.ViewportToWorldPoint(new Vector3(1, 1, 0)).x - 0.5f;
        if (Mathf.Abs(pos.x) > maxHorizontalDistance) {
            vel = 0;
            pos.x = Mathf.Clamp(pos.x, -maxHorizontalDistance, maxHorizontalDistance);
        }
        
        transform.position = pos;
        
        // Take in player's intent.
        targetVel = Input.GetAxisRaw(hAxis) * speed;
        sign = Mathf.Sign(targetVel - vel);

        // Handle case where velocity changes are instant.
        if (Single.IsPositiveInfinity(acc)) {
            vel = targetVel;
            return;
        }
        
        // Velocity Verlet: Calculate new acceleration.
        newAcc = Mathf.Approximately(targetVel, vel) ? 0 : sign * acc;
        
        // Velocity Verlet: Update velocity based on v += 0.5*(a_old * a_new)*t
        vel += 0.5f * (currentAcc + newAcc) * Time.deltaTime;
        
        // Don't overshoot target velocity. Cap it accordingly.
        vel = sign < 0 ? Mathf.Max(vel, targetVel) : Mathf.Min(targetVel, vel);

        currentAcc = newAcc;
    }
}
