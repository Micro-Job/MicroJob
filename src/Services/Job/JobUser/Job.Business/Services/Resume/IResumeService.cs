﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Job.Business.Dtos.ResumeDtos;

namespace Job.Business.Services.Resume
{
    public interface IResumeService
    {
        Task CreateAsync(ResumeCreateDto resumeCreateDto);
    }
}
