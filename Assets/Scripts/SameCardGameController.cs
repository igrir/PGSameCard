using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class SameCardGameController : MonoBehaviour
{

    public TextMeshProUGUI ScoreText;
    public Button PlayButton;


    List<Card> CardList;

    public GameObject CardHolder;

    Coroutine FlipDownRoutine;
    Coroutine CardCheckRoutine;

    Card lastCard;

    int score = 0;

    bool gameReady = false;
    bool isFlipping = false;

    // Use this for initialization
    void Start()
    {
        CardList = new List<Card>(CardHolder.GetComponentsInChildren<Card>());
        // listen touch
        // init card indices
        for (int i = 0; i < CardList.Count; i++)
        {
            CardList[i].OnTouched += CardListOnTouched;
        }


        // init card indices
        for (int i = 0, cardIndex = 0; i < CardList.Count; i += 2, cardIndex++)
        {
            CardList[i].Init(cardIndex);
            CardList[i + 1].Init(cardIndex);
        }

        ShuffleCard();

        PlayButton.onClick.AddListener(Play);
    }

    public void Play()
    {
        InitGames();
    }

    void InitGames()
    {
        gameReady = false;
        isFlipping = false;

        UpdateScore(0);

        if (FlipDownRoutine != null)
            StopCoroutine(FlipDownRoutine);
        FlipDownRoutine = StartCoroutine(IEFlipDownRoutine(onEnd: GameReady));
    }


    IEnumerator IEFlipDownRoutine(Action onEnd)
    {

        // flip down
        for (int i = 0; i < CardList.Count; i++)
        {
            CardList[i].FlipDown(false);
            yield return new WaitForSeconds(0.01f);
        }
        yield return null;

        ShuffleCard();

        // flip up
        for (int i = 0; i < CardList.Count; i++)
        {
            CardList[i].FlipUp(false);
            yield return new WaitForSeconds(0.01f);
        }
        yield return null;

        yield return new WaitForSeconds(1);

        // flup down
        for (int i = 0; i < CardList.Count; i++)
        {
            CardList[i].FlipDown(false);
            yield return new WaitForSeconds(0.01f);
        }
        yield return null;


        onEnd();
    }

    public void GameReady()
    {
        gameReady = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ShuffleCard()
    {
        for (int i = 0; i < CardList.Count; i++)
        {

            int randomIndex = UnityEngine.Random.Range(0, CardList.Count);
            Card card1 = CardList[i];

            Card card2 = CardList[randomIndex];
            Swap(ref card1, ref card2);
        }
    }

    void Swap(ref Card card1, ref Card card2)
    {
        int tmpId;

        tmpId = card1.Id;
        card1.Id = card2.Id;
        card2.Id = tmpId;

        card1.Init(card1.Id);
        card2.Init(card2.Id);
    }

    void CardListOnTouched(Card card)
    {

        if (gameReady)
        {
            // prevent another click
            if (!isFlipping)
            {
                Debug.Log(card.Id);
                if (CardCheckRoutine != null)
                    StopCoroutine(CardCheckRoutine);
                CardCheckRoutine = StartCoroutine(IECardCheck(card));
            }
        }

    }

    IEnumerator IECardCheck(Card card)
    {
        if (!gameReady || isFlipping || card.answered)
            yield break;

        // prevent another click
        isFlipping = true;

        card.FlipUp(false);

        if (lastCard != null)
        {
            if (lastCard != card)
            {

                yield return new WaitForSeconds(0.1f);

                if (card.Id == lastCard.Id)
                {
                    Score();
                    lastCard.answered = true;
                    card.answered = true;
                }
                else
                {
                    Fail();
                    yield return new WaitForSeconds(0.5f);

                    lastCard.FlipDown(false);
                    card.FlipDown(false);
                }

                // reset touch
                lastCard = null;
            }
            // check same
        }
        else
        {
            lastCard = card;
        }

        isFlipping = false;
        yield return null;
    }

    void Score()
    {
        score++;
        UpdateScore(score);

        if (score >= CardList.Count / 2)
        {
            ScoreText.text = "YOU WIN";
        }
    }

    void UpdateScore(int score)
    {
        this.score = score;
        ScoreText.text = score.ToString();
    }

    void Fail()
    {
    }
}
