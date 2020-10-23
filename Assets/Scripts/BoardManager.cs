using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public TileMapGen TMG;

    // Start is called before the first frame update
    void Start()
    {
        TMG.Generate();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
