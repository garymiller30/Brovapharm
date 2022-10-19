using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvapharmNext.Commons
{
    public static class Notify
    {
        private static NotificationManager _notificationManager = new NotificationManager();

        public static void Information(string message)
        {
            Show(message, NotificationType.Information);
        }

        public static void Error(string message)
        {
            Show(message, NotificationType.Error);
        }

        static void Show(string message, NotificationType notificationType)
        {
            _notificationManager.Show(new NotificationContent
            {
                Title = "Бровафарм",
                Message = message,
                Type = notificationType
            }, "WindowArea", TimeSpan.FromSeconds(2));
        }
    }
}
