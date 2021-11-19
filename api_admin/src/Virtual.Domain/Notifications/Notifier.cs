using System.Collections.Generic;
using System.Linq;
using Virtual.Domain.Interfaces;

namespace Virtual.Domain.Notifications
{
    public class Notifier : INotifier
    {
        private readonly List<Notification> _notifications;

        public Notifier()
        {
            _notifications = new List<Notification>();
        }
        public List<Notification> GetNotifications()
        {
            return _notifications;
        }

        public void Handle(Notification notification)
        {
            _notifications.Add(notification);
        }

        public bool IsNotification()
        {
            return _notifications.Any();
        }
    }
}
