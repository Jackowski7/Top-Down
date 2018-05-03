using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerDetector : MonoBehaviour
{

    public bool disableControllers;

    public bool  Xbox_One_Controller = false;
    public bool PS4_Controller = false;

    void Start()
    {
        if (disableControllers == false)
        {

            string[] names = Input.GetJoystickNames();
            for (int x = 0; x < names.Length; x++)
            {
                if (names[x].Length == 19)
                {
                    PS4_Controller = true;
                    Xbox_One_Controller = false;
                }
                if (names[x].Length == 33)
                {
                    //set a controller bool to true
                    PS4_Controller = false;
                    Xbox_One_Controller = true;
                }
            }

        }
    }
}
