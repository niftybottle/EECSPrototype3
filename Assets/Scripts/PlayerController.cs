using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float x;
	public float y;
	public float z;
	public float speed;
	public float dy;
	public float dx;
	public GameObject cam;
	public SceneControlScript scs;
	public float inputx;
	public float inputy;

	// Use this for initialization
	void Start () {
		x = transform.position.x;
		y = transform.position.y;
		z = transform.position.z;
		speed = 1;
		cam = GameObject.Find("Main Camera");
		scs = GameObject.Find ("SceneControl").GetComponent(typeof(SceneControlScript)) as SceneControlScript;
	}
	
	// Update is called once per frame
	void Update () {
		if(!scs.Convo){
			inputy = Input.GetAxis("Vertical");
			inputx = Input.GetAxis("Horizontal");
			dy = speed*inputy;
			dx = speed*inputx;
			transform.Translate(dx,dy,0);
			cam.transform.Translate(dx,dy,0);
			x = transform.position.x;
			y = transform.position.y;
			z = transform.position.z;
		}

	}
}
