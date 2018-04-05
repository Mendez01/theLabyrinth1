using System;
using UnityEngine;

/// <summary>
/// Author : Hwan Kim
/// </summary>
public class MovementSoundV2 : StateMachineBehaviour
{
    private double runSecInterval = 0.5f, walkSecInterval = 1;
    private double lastPlayedSec;

    private double getCurrentSec()
    {
        return TimeSpan.FromMilliseconds(System.DateTime.Now.Millisecond).TotalSeconds;
    }

    private bool isReady(double duration)
    {
        return getCurrentSec() - lastPlayedSec >= duration;
    }

    private void playMovementSound(Animator animator)
    {
        if (animator.GetFloat("Movement_Speed") == 1.0f)
        {
            BattleSoundsManager.instance.playSound(BattleSoundsManager.instance.walkingSound.GetRandomElement<AudioClip>(), 0.1f);
        }
        else
        {
            BattleSoundsManager.instance.playSound(BattleSoundsManager.instance.walkingSound.GetRandomElement<AudioClip>(), 0.25f);
        }
        lastPlayedSec = getCurrentSec();
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playMovementSound(animator);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (isReady(walkSecInterval))
        {
            playMovementSound(animator);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}