using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindController : MonoBehaviour {

    public SteamVR_Controller.DeviceRelation relation;

    public delegate void ControllerFoundEvent();
    public event ControllerFoundEvent OnControllerFound;

    SteamVR_Controller.Device controller;

    public SteamVR_Controller.Device Controller {
        get {
            return controller;
        }
    }

    // Use this for initialization
    IEnumerator Start () {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);

            if (controller != null)
            {
                break;
            }

            int leftIndex = SteamVR_Controller.GetDeviceIndex(relation);
            if (leftIndex >= 0)
            {
                controller = SteamVR_Controller.Input(leftIndex);
                if (OnControllerFound != null) {
                    OnControllerFound();
                }
            }
        }
    }
}
