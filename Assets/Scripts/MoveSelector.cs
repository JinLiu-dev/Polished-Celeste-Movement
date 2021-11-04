using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelector : MonoBehaviour
{
    public float moveStyle;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting on Movement basic");
        GetComponent<Movement>().enabled = true;
        GetComponent<MovementPolish>().enabled = false;
        GetComponent<MovementDistinct>().enabled = false;
        moveStyle = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("1"))
        {
            Debug.Log("Movement basic");
            GetComponent<Movement>().enabled = true;
            GetComponent<MovementPolish>().enabled = false;
            GetComponent<MovementDistinct>().enabled = false;
            moveStyle = 1;
        }

        if(Input.GetKeyDown("2"))
        {
            Debug.Log("Movement Polished");
            GetComponent<Movement>().enabled = false;
            GetComponent<MovementPolish>().enabled = true;
            GetComponent<MovementDistinct>().enabled = false;
            moveStyle = 2;
        }

        if(Input.GetKeyDown("3"))
        {
            Debug.Log("Movement Distinct");
            GetComponent<Movement>().enabled = false;
            GetComponent<MovementPolish>().enabled = false;
            GetComponent<MovementDistinct>().enabled = true;
            moveStyle = 3;
        }
    }
}
