﻿namespace WebApp.Infrastructure.Models;

public class User
{
	public required string Id { get; set; }

	public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}