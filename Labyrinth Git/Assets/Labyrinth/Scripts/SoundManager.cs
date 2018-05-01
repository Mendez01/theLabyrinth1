using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {

    private GameObject[] Enemies;

    AudioSource Intense_Theme;

    public bool play_intense_music = false;

    public bool music_playing = false;

    public bool enemy_nearby;

    private bool[] is_nearby_enemies_array;
	
	void Start () {

        // get all of the enemies 

        Enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Intense_Theme = GetComponent<AudioSource>();

        /*
        Debug.Log(Enemies);

        for (int i = 0; i < Enemies.Length; i++)
        {
            Debug.Log(Enemies[i].GetComponent<CustumEnemyNPC>().player_nearby);
        }
        */
    }
	
	// Update is called once per frame
	public void Update () {

        // check to see if there are any enemies nearby 

        if (Enemies != null)
        {
            
            is_nearby_enemies_array = new bool[Enemies.Length];

            for (int i = 0; i < Enemies.Length; i++)
            {

                // Debug.Log(Enemies[i]);

                // if there are still enemies on game
                // search through the list of enemies 

                if ((play_intense_music == false) && (music_playing == false))
                {
                    // get all the true and false statemtents to see if there is anyone nearby 
                    is_nearby_enemies_array[i] = Enemies[i].GetComponent<CustumEnemyNPC>().player_nearby;

                    // Debug.Log(play_intense_music);
                }
            }

            // assume there is no one neaby (this can change if there is at least one)
            // in the for loop we check if there is at least one

            // enemy_nearby = false;

            // we have a list now so go through the bool list to see if there is 
            // any that are true 

            for (int i = 0; i < is_nearby_enemies_array.Length; i++)
            {
                
                // if one of them are nearby 
                if (is_nearby_enemies_array[i] == true)
                {
                    // Debug.Log(is_nearby_enemies_array[i]);

                    // set the bool to true 
                    enemy_nearby = true;
                }
                
            }



            // if there is an enemy nearby play the intense music

            play_intense_music = enemy_nearby;

            // we have checked through all of the enemies and their bools 
            // if there is one enemy neaby at least and the music has 
            // not been played play the music

            if ((play_intense_music == true) && (music_playing == false))
            {
                Intense_Theme.Play();
                music_playing = true;
            }

            // if there is music playing and no one is nearby 
            // shut it off
            if ((music_playing == true) && (play_intense_music == false))
            {
                Intense_Theme.Stop();
                music_playing = false;
            }
        }

        // if there are no enemies never play instense music

       else if (Enemies == null) {

            // turn off play intense music 

            play_intense_music = false;

       }

	}

}
