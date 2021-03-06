using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputfieldHolder : MonoBehaviour
{
    public TMP_InputField field;
    // Start is called before the first frame update
    void Start()
    {
        //make the input field invisible at first

        field.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
