using System;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class RocketController : MonoBehaviour {

    [SerializeField] private float speed = 3f;
    [SerializeField] private float timeToTopSpeed = 2f;
    [SerializeField] private string hAxis;
    [SerializeField] private string vAxis;
    [SerializeField] private float maxTiltDegrees = 15;

    private Vector2 vel;
    private float acc;
    private Vector2 targetVel;
    private Vector2 currentAcc;
    private Vector2 newAcc;
    private float signX, signY;
    private Camera cam;
    private bool forceUp = false;

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
        float percentage = (speed - vel.x) / (speed * 2);
        transform.eulerAngles = new Vector3(0, 0, percentage * maxTiltDegrees * 2 - maxTiltDegrees);
    }

    private void UpdateRocketMovement() {
        // Velocity Verlet: Update position based on x += vt + 0.5*at^2
        Vector2 pos = transform.position;
        pos += vel * Time.deltaTime + 0.5f * currentAcc * Time.deltaTime * Time.deltaTime;

        // Don't let the rocket go out of camera view.
        // (TODO: This is brittle because it depends on the camera being centered at 0,0.)
        // (TODO: This doesn't need to be calculated every frame.)
        Vector2 maxDistance = cam.ViewportToWorldPoint(new Vector3(1, 1, 0)) + new Vector3(- 0.5f, -1f, 0);
        if (Mathf.Abs(pos.x) > maxDistance.x) {
            vel.x = 0;
            pos.x = Mathf.Clamp(pos.x, -maxDistance.x, maxDistance.x);
        }

        if (Mathf.Abs(pos.y) > maxDistance.y) {
            vel.y = 0;
            pos.y = Mathf.Clamp(pos.y, -maxDistance.y, maxDistance.y);
        }
        
        transform.position = pos;
        
        // Take in player's intent.
        targetVel.x = Input.GetAxisRaw(hAxis) * speed;
        targetVel.y = (forceUp ? 100 : Input.GetAxisRaw(vAxis)) * speed;
        
        // Handle case where velocity changes are instant.
        if (Single.IsPositiveInfinity(acc)) {
            vel = targetVel;
            return;
        }
        
        // Velocity Verlet: Calculate new acceleration.
        signX = Mathf.Sign(targetVel.x - vel.x);
        newAcc.x = Mathf.Approximately(targetVel.x, vel.x) ? 0 : signX * acc;
        signY = Mathf.Sign(targetVel.y - vel.y);
        newAcc.y = Mathf.Approximately(targetVel.y, vel.y) ? 0 : signY * (forceUp ? acc * 7 : acc);
        
        // Velocity Verlet: Update velocity based on v += 0.5*(a_old * a_new)*t
        vel += 0.5f * (currentAcc + newAcc) * Time.deltaTime;
        
        // Don't overshoot target velocity. Cap it accordingly.
        vel.x = signX < 0 ? Mathf.Max(vel.x, targetVel.x) : Mathf.Min(targetVel.x, vel.x);
        vel.y = signY < 0 ? Mathf.Max(vel.y, targetVel.y) : Mathf.Min(targetVel.y, vel.y);

        currentAcc = newAcc;
    }

    public void OverrideUpInput() {
        forceUp = true;
    }
}
