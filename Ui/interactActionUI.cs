using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UIElements;
using UnityEngine.UI;
public class interactActionUI : InteractAction
{
    // Start is called before the first frame update

    public Sprite defaultImage;
    public Sprite actionImage;

    Image imageHolder;
    protected void Start()
    {
        imageHolder = gameObject.GetComponent<Image>();
    }

    public override void interact(){
        imageHolder.sprite = actionImage;
    }

     public override void reset(){
        imageHolder.sprite = defaultImage;
    }
}
