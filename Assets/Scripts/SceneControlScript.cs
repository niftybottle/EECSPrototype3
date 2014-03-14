using UnityEngine;
using System.Collections;

public class SceneControlScript : MonoBehaviour {
	public bool despawnTrigger;
	public bool Convo;
	// Use this for initialization
	void Start () {
		despawnTrigger = false;
		Convo = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void SetDespawn(bool val){
		despawnTrigger = val;
	}
	public void SetConvo(bool val){
		Convo = val;
	}
}
