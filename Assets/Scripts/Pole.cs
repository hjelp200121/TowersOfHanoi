using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pole : MonoBehaviour {
    public Stack<Disc> discs;
    public Vector3 discLocation;
    public Pole next;
    public Pole previous;
    void Awake() {
        discs = new Stack<Disc>();
    }

    // Update is called once per frame
    void Update() {
        
    }
}
