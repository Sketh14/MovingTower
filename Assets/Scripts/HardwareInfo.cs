using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moving_Tower
{
    public class HardwareInfo : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            print(SystemInfo.graphicsDeviceName);
        }
    }
}