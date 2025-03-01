using System.Collections;
using System.Collections.Generic;
using DanielLochner.Assets.SimpleSideMenu;
using UnityEngine;

public class Hover : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] SimpleSideMenu sideMenu;
    [SerializeField] int y_threshold;
    [SerializeField] int x_left_threshold;
    [SerializeField] int x_right_threshold;

    void Update()
    {
        if (y_threshold == 0) Debug.Log("Please enter a y threshold in the respective field of the 'Hover' script ");
        else{
            x_right_threshold = Screen.width;
            if (Input.mousePosition.y <= y_threshold && 
                Input.mousePosition.x >= x_left_threshold &&
                Input.mousePosition.x <= x_right_threshold) 
                    sideMenu.Open();
            else sideMenu.Close();
            
        }
    }
}
