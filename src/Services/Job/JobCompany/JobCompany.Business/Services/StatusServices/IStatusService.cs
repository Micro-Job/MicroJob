﻿using JobCompany.Business.Dtos.StatusDtos;

namespace JobCompany.Business.Services.StatusServices
{
    public interface IStatusService
    {
        //Task CreateStatusAsync(CreateStatusDto dto);
        //Task UpdateStatusAsync(List<UpdateStatusDto> dtos);
        //Task DeleteStatusAsync(string statusId);

        Task<List<StatusListDto>> GetAllStatusesAsync();

        Task ChangeSatusOrderAsync(List<ChangeStatusOrderDto> dto);

        Task ToggleChangeStatusVisibilityAsync(string statusId);
    }
}