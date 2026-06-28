using System;
using Godot;

public partial class NotificationLabel : Label
{
    [Signal] public delegate void NotificationClosedEventHandler();

    [Export] public ColorRect BackgroundRect;

    Timer LifetimeTimer;

    public float Lifetime = 3f;
    public bool AnimateLifetimeBar = false;

    public override void _Ready()
    {
        SetupTimer();

        if (AnimateLifetimeBar)
            StartLifetimeBarAnimation();
    }

    private void SetupTimer()
    {
        LifetimeTimer = new Timer
        {
            WaitTime = Lifetime,
            OneShot = true
        };
        AddChild(LifetimeTimer);
        LifetimeTimer.Timeout += OnTimerTimeout;
        LifetimeTimer.Start();
    }

    private void StartLifetimeBarAnimation()
    {
        var tween = CreateTween();

        tween.TweenProperty(
            BackgroundRect,
            "size",
            new Vector2(0, BackgroundRect.Size.Y),
            Lifetime
        )
        .SetTrans(Tween.TransitionType.Linear);
    }

    public void SetMessage(string message) => Text = message;
    void OnTimerTimeout() => EmitSignal(SignalName.NotificationClosed);
}
