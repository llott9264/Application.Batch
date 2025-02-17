﻿using Application.Batch.Core.Domain.Common;

namespace Application.Batch.Core.Domain.Entities;

public class Address : Entity
{
	public string Street { get; set; } = string.Empty;
	public string City { get; set; } = string.Empty;
	public string State { get; set; } = string.Empty;
	public string ZipCode { get; set; } = string.Empty;
}