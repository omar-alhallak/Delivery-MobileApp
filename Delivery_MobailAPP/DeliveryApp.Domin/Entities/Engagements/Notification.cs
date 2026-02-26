using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryApp.Domain.Entities.Identity;

namespace DeliveryApp.Domain.Entities.Feedback
{
    public class Notification
    {

        public Guid NotificationID { get; private set; }

        public Guid UserId { get; private set; }
        public User? User { get; private set; }


        public string Title { get; private set; } = string.Empty;


        public string Body { get; private set; } = string.Empty;


        public int Type { get; private set; }

        public int? RelatedEntityType { get; private set; }

        public Guid? RelatedEntityID { get; private set; }
        public User? RelatedEntity { get; private set; }


        public bool IsRead { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private Notification() { }
        public Notification(Guid userId, string title, string body, int type, int? relatedEntityType = null, Guid? relatedEntityId = null)
        {
            NotificationID = Guid.NewGuid();
            UserId = userId;
            Title = title;
            Body = body;
            Type = type;
            RelatedEntityType = relatedEntityType;
            RelatedEntityID = relatedEntityId;
            IsRead = false;
            CreatedAt = DateTimeOffset.UtcNow;
        }
        public void MarkAsRead()
        {
            IsRead = true;
        }

        public void UpdateContent(string newTitle, string newBody)
        {
            var normalizedTitle = newTitle;
            if (string.IsNullOrEmpty(normalizedTitle) || normalizedTitle.Length < 3)
                throw new ArgumentException("Notification title must be at least 3 characters.");
            if (normalizedTitle.Length > 150)
                throw new ArgumentException("Title is too long (Max 150 characters).");
            var normalizedBody = newBody;
            if (string.IsNullOrEmpty(normalizedBody))
                throw new ArgumentException("Notification body cannot be empty.");
            if (normalizedBody.Length > 1000)
                throw new ArgumentException("Body is too long (Max 1000 characters).");
            Title = newTitle;
            Body = newBody;
        }
        public void UpdateRelatedEntity(int? newRelatedEntityType, Guid? newRelatedEntityId)
        {
            if (newRelatedEntityType.HasValue && (!newRelatedEntityId.HasValue))
                throw new ArgumentException("RelatedEntityID is required when RelatedEntityType is specified.");
        }
        public void UpdateType(int newType)
        {
            if (newType <= 0)
                throw new ArgumentException("Notification type must be a positive integer.");
            Type = newType;
        }
    }
        
}