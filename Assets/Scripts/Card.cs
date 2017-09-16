using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Card : MonoBehaviour
{

    public int Id;
    public TextMeshPro CardText;

    public float FlipDuration = 0.2f;

    Vector3 flipDown = new Vector3(0, 180, 0);
    Vector3 flipUp = new Vector3(0, 0, 0);


    public delegate void _OnTouched(Card card);
    public _OnTouched OnTouched;

    public bool answered;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(int id)
    {
        CardText = this.GetComponentInChildren<TextMeshPro>();

        this.Id = id;
        this.CardText.text = id.ToString();
    }

    public void FlipUp(bool isImmediate)
    {
        Flip(false, isImmediate);
    }

    public void FlipDown(bool isImmediate)
    {
        Flip(true, isImmediate);
    }

    public void Flip(bool isDown, bool isImmediate = false)
    {

        if (isImmediate)
        {
            if (isDown)
            {
                // flip down
                this.transform.rotation = Quaternion.Euler(flipDown);
            }
            else
            {
                // flip up
                this.transform.rotation = Quaternion.Euler(flipUp);
            }
        }
        else
        {
            // use tween
            if (isDown)
            {
                // flip down
                this.transform.DORotate(flipDown, FlipDuration).SetEase(Ease.OutQuart);
            }
            else
            {
                // flip up
                this.transform.DORotate(flipUp, FlipDuration).SetEase(Ease.InQuart);
            }
        }
    }

    private void OnMouseDown()
    {
        if (OnTouched != null)
            OnTouched(this);
    }
}
