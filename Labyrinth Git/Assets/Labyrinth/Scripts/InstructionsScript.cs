using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InstructionsScript : MonoBehaviour {

    private Text Instructions_Text;

	// Use this for initialization
	void Start () {

        Instructions_Text = this.GetComponent<Text>();

        Instructions_Text.text = "Escape the maze. Avoid the Minotaur at all cost";

        Invoke("Instructions1", 5);

    }

    void Instructions1()
    {
        Instructions_Text.text = "Use W, A, S, D keys to move (hold shift to run)";
        Invoke("Instructions2", 4);
    }
    void Instructions2()
    {
        Instructions_Text.text = "Hold E to attack";

        Invoke("Instructions3", 3);
    }
    void Instructions3()
    {
        Instructions_Text.text = "The spirits can guide you the way";

        Invoke("RemoveText", 4);
    }
    void RemoveText()
    {
        Instructions_Text.text = "";
    }

    public void GameOver()
    {
        Instructions_Text.text = "You were taken by the labyrinth";
    }
    public void Win()
    {
        Instructions_Text.text = "You escaped the labyrinth!";
        Invoke("ExitGame", 4);
    }

    public void ExitGame()
    {
        Instructions_Text.text = "Thanks For Playing!";
        Invoke("CloseApp", 5);
    }

    public void CloseApp()
    {
        Application.Quit();
    }
}
