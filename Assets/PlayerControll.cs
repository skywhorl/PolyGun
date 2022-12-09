using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerControll : MonoBehaviour
{
    public SteamVR_Action_Boolean trigger;
    public GameObject gameObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (trigger.GetStateDown(SteamVR_Input_Sources.Any))
        {
            gameObject.SetActive(true);
        }
    }
}
