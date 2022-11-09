using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumerVisionRadius : MonoBehaviour
{
    public Consumer parent;
    
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.GetComponent<Consumer>();
        this.gameObject.tag = "VisionRadiusCollider";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "VisionRadiusCollider")
        {
            parent.VisionTriggerColliderSawSomething_1Enter_2Stay_3Exit(1, other);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "VisionRadiusCollider")
        {
            parent.VisionTriggerColliderSawSomething_1Enter_2Stay_3Exit(2, other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "VisionRadiusCollider")
        {
            parent.VisionTriggerColliderSawSomething_1Enter_2Stay_3Exit(3, other);
        }
    }
}
