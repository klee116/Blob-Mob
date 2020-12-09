﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deleteOnDone : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(animator.gameObject, stateInfo.length);
    }
}
