using Godot;
using System;
using System.Threading.Tasks;

public partial class Main : Node2D
{
    NotificationManager NotificationManager => GetNode<NotificationManager>("%NotificationManager");

    public override async void _Ready()
    {
        await Task.Delay(2000);
        GD.Print("Showing Notification 1");
        NotificationManager.ShowNotification("Hello World! 1", 10);
        await Task.Delay(2000);
        GD.Print("Showing Notification 2");
        NotificationManager.ShowNotification("Hello World! 2", 10);
    }
}
