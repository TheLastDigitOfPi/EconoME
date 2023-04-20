using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoStartSceneManager : MonoBehaviour
{
    public void StartDemo()
    {
        GlobalSceneManager.Instance.StartDemo();
    }
}
