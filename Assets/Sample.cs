﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SomeNamespace;

public class Sample : MonoBehaviour {
    public TextListItem item;
    public Sprite icon;

    void Start() {
        item.Set(new Contact { 
            name = "vatsal", 
            description = "cool",
            displayPic = icon,
            fontColor = Color.yellow
        });

        item.OnClick += () => Debug.Log("s");
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.Y)){
            item.Set(new Contact {
                name = "ambastha",
                description = "cool",
                displayPic = icon,
                fontColor = Color.red
            });
        }
    }
}
