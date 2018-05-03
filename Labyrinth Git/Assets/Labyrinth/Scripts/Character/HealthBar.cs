using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthBar : MonoBehaviour {
	public Component cc;
	public Image currentHealthbar;
	public Text keyText;
	public float keys = 0;
	private float hp = 100f;
	private float Maxhp = 100f;
	Animator anim;
	public float damage1 = 1;
	public float damage2 = 3;
	public float damage3 = 5;
	public float heal = 30;


	private void Start ()
	{
		UpdateHealthBar();
		UpdateKeys ();
		anim = GetComponent<Animator> ();


	}
		
	void Update()
	{
		UpdateHealthBar();
		UpdateKeys ();
	}

	private void UpdateHealthBar()
	{
		float ration = hp / Maxhp;
		currentHealthbar.rectTransform.localScale = new Vector3 (ration, 1, 1);
	}

	private void UpdateKeys()
	{
		keyText.text = "Keys: " + (keys).ToString() ;
	}
	private void OnTriggerEnter (Collider col)
	{
		if(col.tag == "Weapon"){
			TakeDamage(damage1);
		} 
		if (col.tag == "Weapon2") {
			TakeDamage(damage2);
		}
        
		if (col.tag == "Weapon3") {
			TakeDamage(damage3);
		}
        
		if (col.tag == "Heal") {
			Destroy (col.gameObject);
			HealDamage (heal);
            col.gameObject.GetComponent<AudioSource>().Play();
		}
		if (col.tag == "Key") {
			Destroy (col.gameObject);
			keys += 1;
            col.gameObject.GetComponent<AudioSource>().Play();
		}
        if (col.tag == "Goal")
        {
            anim.SetBool("IsWinning", true);
            GameObject.Find("Instructions_Text").GetComponent<InstructionsScript>().Win();

        }

	}

    /*
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon3")
        {
            TakeDamage(damage3);
        }
    }
    */
    private void TakeDamage (float damage)
	{
		hp -= damage;
		if (hp <= 0) {
			hp = 0;
			//Destroy(anim.gameObject);
			anim.SetBool ("IsDead", true);
            
            Invoke("LoadStartGame", 5);

            GameObject.Find("Instructions_Text").GetComponent<InstructionsScript>().GameOver();
        }
	}

	private void HealDamage (float heal)
	{
		hp += heal;
		if (hp >= Maxhp) {
			hp = Maxhp;
		}
	}

    private void LoadStartGame()
    {
        SceneManager.LoadScene("StartScreen");
    }
}
