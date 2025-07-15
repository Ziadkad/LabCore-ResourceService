﻿using System.Runtime.Serialization;

namespace ResourceService.Application.Common.Models;

public enum UserRole
{
    None,
    
    [EnumMember(Value = "Researcher")]
    Researcher,
    
    [EnumMember(Value = "Manager")]
    Manager,

    [EnumMember(Value = "Admin")]
    Admin,
    
    [EnumMember(Value = "ResourceManager")]
    ResourceManager
}