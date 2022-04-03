using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persist : MonoBehaviour {

    private static GameObject instance;
    
    private void Awake() {
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = gameObject;
        } else {
            Destroy(gameObject);  
        }
        
    }
}
