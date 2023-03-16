using UnityEditor.Animations;
using UnityEngine;

public class GunControls : MonoBehaviour
{
    [SerializeField] private bool rotate, move;
    [SerializeField] private Transform targetObject;

    // Update is called once per frame
    void Update()
    {
        if (rotate)
        {
            transform.LookAt(targetObject);
            //transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }
}
