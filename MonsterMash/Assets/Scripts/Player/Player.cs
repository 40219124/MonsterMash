using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : OverworldAgent
{

    // Update is called once per frame
    void Update()
    {
        HorizontalValue = Input.GetAxisRaw("Horizontal");
        VerticalValue = Input.GetAxisRaw("Vertical");

        DoUpdate();

    }

}
