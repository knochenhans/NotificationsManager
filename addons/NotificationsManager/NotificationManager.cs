using Godot;
using System.Collections.Generic;

public partial class NotificationManager : MarginContainer
{
    [Export] public PackedScene NotificationScene;
    
    [Export] public int MaxNotifications = 5;
    [Export] public int NotificationSpacing = 0;

    [ExportCategory("Lifetime")]
    [Export] public float NotificationLifetime = 3f;
    [Export] public bool AnimateLifetimeBar = false;
    
    [ExportCategory("Movement")]
    [Export] public float NotificationMoveSpeed = 1f;
    [Export] public Vector2 MoveDirection = new(0, 0);

    [ExportCategory("Fade")]
    [Export] public float NotificationFadeDuration = 1f;

    private Queue<NotificationLabel> NotificationQueue = new();
    private Control NotificationContainer => GetNode<Control>("NotificationContainer");

    public override void _Ready()
    {
        Logger.Log("NotificationManager is ready", "NotificationManager", Logger.LogTypeEnum.UI);
    }

    public void ShowNotification(string message, float lifetime = -1f)
    {
        var notification = NotificationScene.Instantiate<NotificationLabel>();
        notification.SetMessage(message);
        notification.Lifetime = lifetime > 0 ? lifetime : NotificationLifetime;
        notification.TreeExited += () => NotificationQueue.Dequeue();
        notification.NotificationClosed += () => OnNotificationClosed(notification);

        NotificationContainer.AddChild(notification);

        NotificationQueue.Enqueue(notification);
        // if (NotificationQueue.Count > MaxNotifications)
        // {
        //     var oldestNotification = NotificationQueue.Dequeue();
        //     oldestNotification.QueueFree();
        // }

        notification.GlobalPosition = new Vector2(0, (NotificationContainer.GetChildCount() - 1) * notification.Size.Y);
    }

    private async void OnNotificationClosed(NotificationLabel notification)
    {
        if (MoveDirection != Vector2.Zero)
        {
            var tween = CreateTween();
            var offset = new Vector2(MoveDirection.X * notification.Size.X,
                         MoveDirection.Y * notification.Size.Y);
            var targetPos = notification.Position + offset;
            tween.TweenProperty(notification, "position", targetPos, NotificationMoveSpeed)
                 .SetTrans(Tween.TransitionType.Cubic)
                 .SetEase(Tween.EaseType.Out);
            await ToSignal(tween, "finished");
        }

        if (NotificationFadeDuration > 0)
        {
            var fadeTween = CreateTween();
            fadeTween.TweenProperty(notification, "modulate:a", 0f, NotificationFadeDuration)
                      .SetTrans(Tween.TransitionType.Cubic)
                      .SetEase(Tween.EaseType.Out);
            await ToSignal(fadeTween, "finished");
        }

        notification.QueueFree();
    }
}
