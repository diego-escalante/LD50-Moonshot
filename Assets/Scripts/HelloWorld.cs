using UnityEngine;

public class HelloWorld : MonoBehaviour {
    void Start() {
        Debug.Log("Hello World!");
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            GetComponent<Camera>().backgroundColor = new Color(Random.value, Random.value, Random.value);
        }
    }
}
