using UnityEngine;
using System.Collections;

public class mInput : MonoBehaviour {
    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;
	// Use this for initialization
	void Start () {
        mainCamera = Camera.main;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
                Debug.Log(hit.collider.name);
        }
        //{
        //    Physics2D.Raycast(mainCamera.transform.position, new Vector2(mainCamera.ScreenToWorldPoint(Input.mousePosition).x, mainCamera.ScreenToWorldPoint(Input.mousePosition).y),100);
        //    Debug.Log(hit.collider.name);
        //}
        //{
        //    //hit = Physics2D.Linecast(mainCamera.transform.position, new Vector2(mainCamera.ScreenToWorldPoint(Input.mousePosition).x, mainCamera.ScreenToWorldPoint(Input.mousePosition).y));
        //    Debug.Log(mainCamera.ScreenToWorldPoint(Input.mousePosition));
        //    Debug.Log(hit.collider.name);
        //}

        ;
	}
}
