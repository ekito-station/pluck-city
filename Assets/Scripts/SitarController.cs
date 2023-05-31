using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SitarController : StringController
{
    public Material gold;
    public Material gold1;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "MainCamera")
        {
            GetComponent<MeshRenderer>().material = gold1;
            if (IsInvoking("ReturnMaterial")) CancelInvoke();            
            Invoke("ReturnMaterial", returnTime);
        }
    }

    private void ReturnMaterial()
    {
        GetComponent<MeshRenderer>().material = gold;
    }    
}
