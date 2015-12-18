using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fighter : MonoBehaviour {
	public Text stats;
	public Text nameBar;
	public GameObject hpBar;
	public string name = "Gerard";

	private float hitProb = .5f;
	private float hp = 1;
	private float defense = 0;
	private float strength = .2f;
	// Use this for initialization
	void Start () {
		nameBar.text = name;
	}
	
	// Update is called once per frame
	void Update () {

	}
		
	public float getHP() {
		return hp;
	}

	public bool isDead() {
		return hp <= 0;
	}

	public void hit(float amount) {
		hp -= amount * (1 - defense);
		defense -= (amount / 4);
		if (defense < 0)
			defense = 0;
		if (hp <= 0)
			hp = 0;
		UpdateStats ();
	}

	public float getHitProb() {
		return hitProb;
	}

	public float getDefense() {
		return defense;
	}

	public float getStrength() {
		return strength;
	}

	public void powerUp() {
		defense += .01f;
		if (defense >= .75f)
			defense = .75f;
		strength += .05f;
		hitProb -= .05f;
		if (hitProb <= .15f) {
			hitProb = .15f;
		}
		UpdateStats ();
	}

	public void defend() {
		defense += .05f;
		if (defense >= .75f) {
			defense = .75f;
		}
		UpdateStats ();
	}

	public void UpdateStats() {
		hpBar.transform.localScale = new Vector3 (hp, 1, 1);
		stats.text = "Strength = " + (int)(strength * 100) + "\n"
			+ "Hit % = " + (int)(hitProb * 100) + "\n"
			+ "Defense = " + (int)(defense * 100);
	}
}
