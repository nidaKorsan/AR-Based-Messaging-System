using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class textManager : MonoBehaviour
{
    public static textManager current;
    private int count = 0;
    private void Awake()
    {
        current = this;
    }

    public void write(string some)
    {
        if (count < 10)
        {
            GetComponent<Text>().text += some + "\n";
            ++count;
        }
        else
        {
            GetComponent<Text>().text = some + "\n";
            count = 0;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
