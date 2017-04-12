using UnityEngine;
using System.Collections;

public class LoadGame : MonoBehaviour {
	// Use this for initialization
    public GameObject gameManager;
    void Awake()
    {
        if (GameControl.Instance == null)
            GameObject.Instantiate(gameManager);
    }

	
	// Update is called once per frame
	void Update () {
	
	}
}
