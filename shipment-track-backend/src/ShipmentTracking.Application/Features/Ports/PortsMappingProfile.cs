using AutoMapper;
using ShipmentTracking.Application.Features.Ports.Dto;
using ShipmentTracking.Domain.Entities;

namespace ShipmentTracking.Application.Features.Ports;

/// <summary>
/// AutoMapper profile for port-related mappings.
/// </summary>
public sealed class PortsMappingProfile : Profile
{
    public PortsMappingProfile()
    {
        CreateMap<PortMaster, PortDto>();
    }
}
