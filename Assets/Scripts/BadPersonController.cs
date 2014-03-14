using UnityEngine;
using System.Collections;

public class BadPersonController : MonoBehaviour {
	public PlayerController pc;
	public float x;
	public float y;
	public Component c;
	public TextAsset ta;
	public bool talking = false;
	public float dist;
	public GameObject controller;
	public DialogueHelper dh;
	// Use this for initialization
	void Start () {
		pc = GameObject.Find("Character").GetComponent("PlayerController") as PlayerController;
		x = transform.position.x;
		y = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		dist = Mathf.Abs(pc.x - x) + Mathf.Abs(pc.y - y);
		if(!talking && dist<10){
			talking = true;
			//c = controller.AddComponent("DialogueHelper");
			dh = (DialogueHelper)controller.GetComponent(typeof(DialogueHelper));
			dh.parseDialogueTree(ta.text);
			dh.start = true;
		}
	}
	void despawn() {
		//DestroyObject(c);
		dh.Restart();
		DestroyObject(gameObject);
	}
}
