using MoreMountains.Feedbacks;
using UnityEngine;

public class Miner : MonoBehaviour
{
    public SpawnPointHolder parent;
    public int level;
    public int income;
    private MMF_Player player;
    private MMF_Player miningFeedBack;
    private Animator anim;
    public AnimationEvent @event;
    public AnimationClip anim2;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim2 = anim.runtimeAnimatorController.animationClips[0];
        player = transform.GetComponent<MMF_Player>();
        AnimationEventAdd();
    }

    private void AnimationEventAdd()
    {
        @event = new AnimationEvent();
        @event.functionName = "Trigger";
        @event.time = 0.75f;
        if (anim2.events.Length == 0)
            anim2.AddEvent(@event);
    }

    private void OnEnable()
    {
        miningFeedBack = parent.feedback;
    }

    public void Trigger()
    {
        MoneySignals.Signal_EarnMoney(income);
        player.GetFeedbackOfType<MMF_FloatingText>().Value = "+" + income.ToString();
        player.PlayFeedbacks();
        if (miningFeedBack == null) miningFeedBack = parent.feedback;
        miningFeedBack.PlayFeedbacks();
    }
}