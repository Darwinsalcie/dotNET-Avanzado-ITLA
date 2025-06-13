using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    public class TodoCreatedEvent
    {
        public int TaskId { get; }
        public string Title { get; }
        public DateTime CreatedAt { get; }

        public TodoCreatedEvent(int taskId, string title, DateTime createdAt)
        {
            TaskId = taskId;
            Title = title;
            CreatedAt = createdAt;
        }
    }
}
