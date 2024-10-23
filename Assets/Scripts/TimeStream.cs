using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TimeStream : MonoBehaviour
{
    float time_ = 0;
    float dtime_ = 0;
    [SerializeField]
    int step = 0;

    public bool endlessGame = true;
    public int numberEpochs = 100;

    [SerializeField]
    Field field;

    Slider evolSpeedSlider_;
    TwoPlayerModeController controller_;
    RectTransform controllerTransform_;
    Vector3 activeControllerPos_;
    Vector3 deactiveControllerPos_;
    float moveControllerSpeed_ = 2f;

    TMP_InputField stepText_;
    [SerializeField] Button randomButton;
    [SerializeField] Button clearButton;
    [SerializeField] Button startStopButton;
    [SerializeField] Button settingsButton;
    [SerializeField] TMP_InputField Score1P, Score2P;

    void Awake() {
        evolSpeedSlider_ = GetComponentInChildren<Slider>();
        if (evolSpeedSlider_ == null) {
            Debug.LogError("Evolution speed slider not found", this);
        }

        controller_ = GetComponentInChildren<TwoPlayerModeController>(true);
        if (controller_ == null) {
            Debug.LogError("2 player mode controller not found", this);
        }
        controllerTransform_ = controller_.GetComponent<RectTransform>();
        activeControllerPos_ = controllerTransform_.localPosition;
        deactiveControllerPos_ = activeControllerPos_;
        deactiveControllerPos_.x = 1700f;

        stepText_ = GetComponentInChildren<TMP_InputField>();
        if (stepText_ == null) {
            Debug.LogError("Feild for write step count not found", this);
        }

        randomButton.interactable = false;
        clearButton.interactable = false;
        startStopButton.interactable = false;
        settingsButton.interactable = false;
        stepText_.interactable = false;
        controller_.gameObject.SetActive(false);
    }

    public void StartGame() {
        randomButton.interactable = true;
        clearButton.interactable = true;
        startStopButton.interactable = true;
        settingsButton.interactable = true;
        stepText_.interactable = true;
        controller_.gameObject.SetActive(true);

        streamStoped = true;
        step = 0;
    }

    bool streamStoped = true;

    public void StartStop() {
        streamStoped ^= true;

        randomButton.interactable = streamStoped;
        clearButton.interactable = streamStoped;
        settingsButton.interactable = streamStoped;
    }

    public bool IsStoped() {
        return streamStoped;
    }

    [SerializeField]
    int evolutionSpeed = 1;
    [SerializeField]
    int kMinEvolutionSpeed = 1;
    [SerializeField]
    int kMaxEvolutionSpeed = 20;

    public void UpdateEvolutionSpeed() {
        evolutionSpeed = kMinEvolutionSpeed + (int)((kMaxEvolutionSpeed - kMinEvolutionSpeed) * evolSpeedSlider_.value);
    }

    void Update()
    {
        if (!streamStoped) {
            time_ += Time.deltaTime;
            dtime_ += Time.deltaTime;
        }

        while (dtime_ >= 1.0f / evolutionSpeed) {
            dtime_ -= 1.0f / evolutionSpeed;
            step += 1;

            stepText_.text = "Step: " + step.ToString();

            field.UpdateLive();
            UpdateStatistics();
        }

        MoveController();
        CheckEndGame();
    }

    int p1Score_, p2Score_;
    void UpdateStatistics() {
        int player1 = field.GetStatistics(1);
        int player2 = field.GetStatistics(2);
        field.ResetStatistics();

        int dscore = Math.Abs(player1 - player2);
        p1Score_ += dscore * (player1 > player2 ? 1 : -1);
        p2Score_ -= dscore * (player1 > player2 ? 1 : -1);

        Score1P.text = "1P Score: " + p1Score_.ToString();
        Score2P.text = "2P Score: " + p2Score_.ToString();
    }

    public void ResetStepScore() {
        step = 0;
        p1Score_ = 0;
        p2Score_ = 0;

        Score1P.text = "1P Score: " + p1Score_.ToString();
        Score2P.text = "2P Score: " + p2Score_.ToString();
        stepText_.text = "Step: " + step.ToString();
    }

    void MoveController() {
        if (!streamStoped) {
            controllerTransform_.localPosition = Vector3.Lerp(
                controllerTransform_.localPosition,
                deactiveControllerPos_,
                Mathf.Min(1.0f, moveControllerSpeed_ * Time.deltaTime)
            );
        } else {
            controllerTransform_.localPosition = Vector3.Lerp(
                controllerTransform_.localPosition,
                activeControllerPos_,
                Mathf.Min(1.0f, moveControllerSpeed_ * Time.deltaTime)
            );
        }
    }

    [SerializeField] EndGame endGame;
    void CheckEndGame() {
        if (endlessGame)
            return;
        
        if (step >= numberEpochs) {
            StartStop();
            endGame.gameObject.SetActive(true);
            endGame.GameEnd(p1Score_, p2Score_);
        }
    }
}
