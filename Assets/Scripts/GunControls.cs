using UnityEditor.Animations;
using UnityEngine;

public class GunControls : MonoBehaviour
{
    [SerializeField] private bool rotate, move;
    [SerializeField] private Transform targetObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rotate)
        {
            transform.LookAt(targetObject);
        }
    }
}
