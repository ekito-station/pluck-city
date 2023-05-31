using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickButton()
    {
        float size = 0.8f;
        this.transform.localScale = new Vector3(size, size, 1.0f);
        Invoke("RecoverButton", 0.1f);
    }

    public void RecoverButton()
    {
        this.transform.localScale = Vector3.one;
    }
}
