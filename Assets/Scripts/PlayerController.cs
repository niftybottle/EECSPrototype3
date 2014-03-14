using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float x;
	public float y;
	public float z;
	public float speed = 1;
	public float dy;
	public float dx;
	public GameObject cam;
	// Use this for initialization
	void Start () {
		x = transform.position.x;
		y = transform.position.y;
		z = transform.position.z;
		cam = GameObject.Find("Main Camera");
	}
	
	// Update is called once per frame
	void Update () {
		dy = speed*Input.GetAxis("Vertical");
		dx = speed*Input.GetAxis("Horizontal");
		transform.Translate(dx,dy,0);
		cam.transform.Translate(dx,dy,0);
		x = transform.position.x;
		y = transform.position.y;
		z = transform.position.z;
	}
}
