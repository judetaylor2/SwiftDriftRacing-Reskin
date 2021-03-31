﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public LayerMask whatIsGround;
    public GameObject explosion, prefabParent;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void DestroyObject()
    {
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(prefabParent);
    }
}
