using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuitarController : StringController
{
    public Material silver;
    public Material silver1;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "MainCamera")
        {
            GetComponent<MeshRenderer>().material = silver1;
            if (IsInvoking("ReturnMaterial")) CancelInvoke();
            Invoke("ReturnMaterial", returnTime);
        }
    }

    private void ReturnMaterial()
    {
        GetComponent<MeshRenderer>().material = silver;
    }
}
