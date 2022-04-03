using System;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioClip explosion;
    public AudioClip hit;
    public AudioClip rocketLaunch;

    public AudioSource audioSource;
    public AudioSource humSource;

    public void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnEnable() {
        EventManager.StartListening(EventManager.Event.RocketHit, playHitSound);
        EventManager.StartListening(EventManager.Event.PreExplosion, playHitSound);
        EventManager.StartListening(EventManager.Event.PreExplosion, StopHum);
        EventManager.StartListening(EventManager.Event.Launching, playrocketLaunchSound);
        EventManager.StartListening(EventManager.Event.Launching, StartHum);
        EventManager.StartListening(EventManager.Event.RocketExplode, playExplosionSound);
    }

    public void OnDisable() {
        EventManager.StopListening(EventManager.Event.RocketHit, playHitSound);
        EventManager.StopListening(EventManager.Event.PreExplosion, playHitSound);
        EventManager.StopListening(EventManager.Event.PreExplosion, StopHum);
        EventManager.StopListening(EventManager.Event.Launching, playrocketLaunchSound);
        EventManager.StopListening(EventManager.Event.Launching, StartHum);
        EventManager.StopListening(EventManager.Event.RocketExplode, playExplosionSound);
    }

    public void playExplosionSound() {
        audioSource.PlayOneShot(explosion, 1);
    }

    public void playHitSound() {
        audioSource.PlayOneShot(hit, 1);
    }

    public void playrocketLaunchSound() {
        audioSource.PlayOneShot(rocketLaunch, 1);
    }

    public void StartHum() {
        humSource.Play();
    }
    
    public void StopHum() {
        humSource.Stop();
    }
}