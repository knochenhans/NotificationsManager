using Godot;
using System;
using System.Threading.Tasks;

public partial class Main : Node2D
{
    NotificationManager NotificationManager => GetNode<NotificationManager>("%NotificationManager");

    public override async void _Ready()
    {
        await Task.Delay(1000);
        GD.Print("Showing Notification 1");
        NotificationManager.ShowNotification("Hello World! 1", 2);
        await Task.Delay(500);
        GD.Print("Showing Notification 2");
        NotificationManager.ShowNotification("Hello World! 2", 2);
        await Task.Delay(500);
        GD.Print("Showing Notification 3");
        NotificationManager.ShowNotification("Hello World! 3", 2);
        await Task.Delay(2000);
        GD.Print("Showing Notification 4");
        NotificationManager.ShowNotification("Hello World! 4", 2);
    }
}
