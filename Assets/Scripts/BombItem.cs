using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombItem : MonoBehaviour, Item
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Activate(List<Character> characters){
      foreach (Character character in characters){
        character.ModifyHealth(-10);
      }
    }
}
