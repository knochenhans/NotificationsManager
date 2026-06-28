using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class NotificationManager : MarginContainer
{
    [Export] public PackedScene NotificationScene;
    [Export] private Control NotificationContainer;

    [Export] public int MaxNotifications = 5;
    [Export] public int NotificationSpacing = 0;

    [ExportGroup("Lifetime")]
    [Export] public float NotificationLifetime = 3f;
    [Export] public bool AnimateLifetimeBar = false;

    [ExportGroup("Movement")]
    [Export] public Vector2 EntryOffset = new(250, 0);
    [Export] public Vector2 ExitOffset = new(250, 0);
    [Export] public float NotificationMoveSpeed = 1f;

    [ExportGroup("Fade")]
    [Export] public float NotificationFadeInDuration = 0.3f;
    [Export] public float NotificationFadeOutDuration = 0.3f;

    public override void _Ready()
    {
        Logger.Log("NotificationManager is ready", "NotificationManager", Logger.LogTypeEnum.UI);
    }

    public async void ShowNotification(string message, float lifetime = -1f)
    {
        var notification = NotificationScene.Instantiate<NotificationLabel>();

        notification.SetMessage(message);
        notification.Lifetime = lifetime > 0 ? lifetime : NotificationLifetime;
        notification.AnimateLifetimeBar = AnimateLifetimeBar;

        notification.NotificationClosed += () => OnNotificationClosed(notification);

        NotificationContainer.AddChild(notification);

        notification.OffsetTransformEnabled = true;
        notification.OffsetTransformVisualOnly = true;

        notification.OffsetTransformPosition = EntryOffset;
        notification.Modulate = new Color(1, 1, 1, 0);

        Logger.Log($"Showing notification: {message}", "NotificationManager", Logger.LogTypeEnum.UI);

        await TweenInOut(notification, true);
    }

    private async Task TweenInOut(NotificationLabel notification, bool tweenIn)
    {
        var tween = CreateTween();
        tween.SetParallel();

        var targetPosition = tweenIn ? Vector2.Zero : ExitOffset;
        var duration = tweenIn ? NotificationFadeInDuration : NotificationFadeOutDuration;
        var ease = tweenIn ? Tween.EaseType.Out : Tween.EaseType.In;
        var targetAlpha = tweenIn ? 1.0f : 0.0f;

        tween.TweenProperty(notification, "offset_transform_position", targetPosition, duration)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(ease);

        tween.TweenProperty(notification, "modulate:a", targetAlpha, duration);

        await ToSignal(tween, Tween.SignalName.Finished);
    }

    private async void OnNotificationClosed(NotificationLabel notification)
    {
        if (!IsInstanceValid(notification))
            return;

        await TweenInOut(notification, false);

        notification.QueueFree();
    }
}
