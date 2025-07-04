﻿using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace Application.DTOs.Response
{
    public class TodoResponseDTO
    {
        public int Id { get;  set; }
        public string Title { get;  set; }
        public string? Description { get;  set; }
        public DateTime CreatedAt { get;  set; }
        public DateTime? DueDate { get;  set; }
        public bool IsDeleted { get;  set; }
        public Status Status { get;  set; }
        public Priority? Priority { get;  set; }
        public string? AdditionalData { get;  set; }
        public int? DaysRemaining { get;  set; }

        // El nombre del campo debe ser diferente al de la propiedad
        [JsonPropertyName("priorityText")]
        public string? PriorityText
            => Priority.HasValue ? Priority.Value.ToString() : null;

        [JsonPropertyName("statusText")]
        public string StatusText
      => Status.ToString();


    }
}
