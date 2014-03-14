using UnityEngine;
using System.Collections;

//This class implements a graphical pair of health bars, and keeps track of two values: health 1 and health 2. Health
//1 is the traditional concept of health, and may also be called "primary health". Health 2 is something like hunger or
//mental health, where if it gets too low, health 1 begins to be eroded.
//The point of health 2 at which health 1 begins to erode is configurable, as is the rate of decay, and the set of
//pictures used for health 1 and 2's health bars, and the labels for the healths. Damaging/healing either health or 
//setting their picture sets are implemented as functions, to maintain dependent values needed for the display
//
//Current limitations:
//-No variable width health bars
//-Decay is a flat rate, not proportional how low health 2 is
//-GUI alignment may not be perfect
//-No text formatting options
//-Many values are simply public, rather than having get/set functions
public class DoubleHealthScript : MonoBehaviour {

	//The totals for health 1 and health 2. If current health = max health, then they are at 100% health
	public float maxH1;
	public float maxH2;
	//Current values for health 1 and 2. Functions are available to increase and decrease these, please use them rather than
	//modifying directly when possible
	public float health1;
	public float health2;

	//Legacy variables from text representation of values
	//public string h1str;
	//public string h2str;

	//Current images for representing health 1 and 2 (please don't modify)
	private Texture2D h1pic;
	private Texture2D h2pic;

	//The set of pictures to use when representing health, in order from 0 health to full health. Please use the provided
	//functions for setting these
	public Texture2D[] h1PicSet;
	public Texture2D[] h2PicSet;
	//These are automatically set; they keep track of the breakpoints in the representation of health
	public int health1DivNum;
	public float health1DivSize;
	public int health1Stage;
	private int health2DivNum;
	private float health2DivSize;
	private int health2Stage;
	public int picwidth;
	public int picheight;

	//The cut-off for health2 damaging health1
	public float tippingPoint;
	//The rate at which health1 deacreases when health2 is below tippingPoint, in points/frame
	public float damageRate;
	//What Health 1 and 2 are labeled as (for example, HP and Hunger)
	public string H1label;
	public string H2label;
	//Whether Health 1 and 2 can exceed 100% via healing (true = can, false = can't)
	public bool softMax;

	public DeathMessage dm;



	// Set values - change these here, or via code (they are public variables)
	void Start () {
		//Set total health for primary (H1) and secondary (H2)
		maxH1 = 100;
		maxH2 = 100;

		//Set current health to full
		health1 = maxH1;
		health2 = maxH2;

		//Set the tipping point where H1 will start to decrease due to H2 being too low (default = 50% of H2)
		tippingPoint = maxH2/2;
		//Set how fast H1 depletes (in damage/frame) after H2 passes the tipping point
		damageRate = .1f;

		//h1str = health1.ToString ("F0");
		//h2str = health2.ToString ("F0");
		setH1PicSet(h1PicSet);
		setH2PicSet(h2PicSet);
		h1pic = h1PicSet[health1Stage];
		h2pic = h2PicSet[health2Stage];
		//At the moment, doesn't fn with variable width health bars - specifically, it will assume that everything
		//is the same width and height as full health, which may look strange or be cut off if this is not the case.
		picwidth = Mathf.Max(h1pic.width, h2pic.width);
		picheight = Mathf.Max(h1pic.height, h2pic.height);

		//Set what Health 1 and Health 2 are labeled as
		H1label = "Health";
		H2label = "Mental Health";

		//Set whether you can exceed the maximum amount of health (by healing) (default = 50%)
		softMax = false;
	}
	//-----------------------------------------------------
	//--- Functions users may want to edit or call ---
	//-----------------------------------------------------

	//Function to call when health reaches zero
	void OnDeath(){
		//Put things you would like to happen when primary health reaches 0 here
		dm.dead = true;
	}
	//Represent damage to Secondary Health. Negative damage heals
	//Input: Magnitude of damage/healing
	//Ex: hithealth2(-23.5) would heal health2 by 23.5 points
	public void hitHealth1(float damage){
		health1 -= damage;
		health1Stage = Mathf.CeilToInt(health1/health1DivSize);
		h1pic = h1PicSet[health1Stage];
		//h1str = health1.ToString ("F0");
	}
	//Represent damage to Secondary Health. Negative damage heals
	//Input: Magnitude of damage/healing
	//Ex: hithealth2(-23.5) would heal health2 by 23.5 points
	public void hitHealth2(float damage){
		health2 -= damage;
		if (health2 <= maxH2){
			health2Stage = Mathf.CeilToInt(health2/health2DivSize);
		}else{
			health2Stage = health2DivNum;
		}

		h2pic = h2PicSet[health2Stage];
		//h2str = health2.ToString ("F0");
	}
	//Sets the set of images that will be used to represent Health 1's current status
	//Input: An array of Texture2Ds to be used, increasing from element 0 = 0 health to last element = 100% health
	//Ex: the default display is an array of size 11, with texture[0] being an empty bar, texture[10] being a full bar,
	//	and texture[5] being a half full bar
	//
	//The intervals at which the display moves from one picture to the next are automatically generated, assuming
	//the hp range for each division is equal in size and rounding up (ie if max health is 100, and there are 11 images 
	//(one being no health), image will change at 0, 10, 20, ... 100, with 15 displaying with 20's image, 21 with 30's,
	//etc.)
	public void setH1PicSet(Texture2D[] texts){
		h1PicSet = texts;
		health1DivNum = h1PicSet.Length-1;
		health1DivSize = maxH1/health1DivNum;
		health1Stage = health1DivNum;
	}
	//Sets the set of images that will be used to represent Health 2's current status
	//Input: An array of Texture2Ds to be used, increasing from element 0 = 0 health to last element = 100% health
	//Ex: the default display is an array of size 11, with texture[0] being an empty bar, texture[10] being a full bar,
	//	and texture[5] being a half full bar
	//
	//The intervals at which the display moves from one picture to the next are automatically generated, assuming
	//the hp range for each division is equal in size and rounding up (ie if max health is 100, and there are 11 images 
	//(one being no health), image will change at 0, 10, 20, ... 100, with 15 displaying with 20's image, 21 with 30's,
	//etc.)
	public void setH2PicSet(Texture2D[] texts){
		h2PicSet = texts;
		health2DivNum = h2PicSet.Length-1;
		health2DivSize = maxH2/health2DivNum;
		health2Stage = health2DivNum;
	}
	//-----------------------------------------------------
	//- End functions users may want to mess with or call -
	//-----------------------------------------------------

	// Update is called once per frame
	void Update () {
		if(health2 < 0){
			health2 = 0;
			health2Stage = 0;
			h2pic = h2PicSet[health2Stage];
			//h2str = health2.ToString ("F0");
		}
		if(health1 <= 0){
			OnDeath ();
		}

		if (softMax && health1 > maxH1){
			health1 = maxH1;
			health2Stage = health2DivNum;
			h2pic = h2PicSet[health2Stage];
			//h2str = health2.ToString ("F0");
		}
		if (softMax && health2 > maxH2){
			health2 = maxH2;
			health2Stage = health2DivNum;
			h2pic = h2PicSet[health2Stage];
			//h2str = health2.ToString ("F0");
		}
		if(health2 < tippingPoint){
			hitHealth1 (damageRate);
		}
	}
	void OnGUI () {
		string h1labelplus = H1label + ": ";
		string h2labelplus = H2label + ": ";
		GUILayout.BeginArea(new Rect(0,0,Mathf.Max(h1labelplus.Length, h2labelplus.Length)*8+picwidth,picheight*2+15));
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		GUILayout.Label(h1labelplus);
		GUILayout.Space(picheight-12);
		GUILayout.Label(h2labelplus);
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		GUILayout.Label(h1pic);
		GUILayout.Label(h2pic);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();

		//GUI.Label(new Rect(0,0,300,25), new GUIContent(H1label+": ", h1pic));
		//GUI.Label(new Rect(0,25,300,25), new GUIContent(H2label+": ", h2pic));
	}


}
