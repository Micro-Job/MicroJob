﻿using FluentValidation;
using SharedLibrary.Enums;

namespace JobCompany.Business.Dtos.SkillDtos;

public class SkillUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}