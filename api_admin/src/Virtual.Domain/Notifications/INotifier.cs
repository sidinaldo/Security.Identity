using System.Collections.Generic;

namespace Virtual.Domain.Notifications
{
    public interface INotifier
    {
        bool IsNotification();
        List<Notification> GetNotifications();
        void Handle(Notification notification);
    }
}
