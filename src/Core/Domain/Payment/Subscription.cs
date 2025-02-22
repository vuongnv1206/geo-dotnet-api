﻿namespace FSH.WebApi.Domain.Payment;
public class Subscription : AuditableEntity, IAggregateRoot
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Duration { get; set; }
    public string Role { get; set; }
    public string Image { get; set; }
}
