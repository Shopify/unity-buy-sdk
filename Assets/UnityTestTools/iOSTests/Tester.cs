using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class Tester : MonoBehaviour {
    [DllImport ("__Internal")]
    protected static extern void _TesterObjectFinishedLoading();

    // Use this for initialization
    void Start () {
        _TesterObjectFinishedLoading();
    }
    
    // Update is called once per frame
    void Update () {
        
    }
}
