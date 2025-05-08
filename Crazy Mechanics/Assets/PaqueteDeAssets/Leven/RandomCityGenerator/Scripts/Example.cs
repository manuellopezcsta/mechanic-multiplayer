using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    public CityGenerator cityGenerator;
    // Start is called before the first frame update
    void Start()
    {
        if(cityGenerator&&cityGenerator.transform.childCount==0)
        {
            cityGenerator.GenerateCity();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(cityGenerator)
                cityGenerator.GenerateCity();
        }
    }
}
