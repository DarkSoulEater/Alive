using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    [SerializeField] TMP_InputField endGame;
    [SerializeField] TMP_InputField P1Score;
    [SerializeField] TMP_InputField P2Score;

    public void GameEnd(int p1Score, int p2Score) {
        endGame.text = "End Game";
        P1Score.text = "P1 Score: " + p1Score.ToString();
        P2Score.text = "P2 Score: " + p2Score.ToString();
    }
    
    public void CloseMenu() {
        gameObject.SetActive(false);
    }
}
