using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Score : MonoBehaviour
{
    public int score = 0;
    private Text scoreText;

    public void Start()
    {
        scoreText = gameObject.GetComponent<Text>();
        IncreaseScore(0);
    }

    public void IncreaseScore(int scoreValue)
    {
        score += scoreValue;
        scoreText.text = "Score: " + score.ToString();
    }

    public int GetScore()
    {
        return score;
    }
}
