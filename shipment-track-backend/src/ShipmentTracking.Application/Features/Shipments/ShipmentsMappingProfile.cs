using System.Linq;
using AutoMapper;
using ShipmentTracking.Application.Features.Shipments.Dto;
using ShipmentTracking.Domain.Entities;

namespace ShipmentTracking.Application.Features.Shipments;

/// <summary>
/// AutoMapper profile for shipment features.
/// </summary>
public sealed class ShipmentsMappingProfile : Profile
{
    public ShipmentsMappingProfile()
    {
        CreateMap<ShipmentStatusHistory, ShipmentStatusHistoryDto>()
            .MaxDepth(1);

        CreateMap<Shipment, ShipmentDto>()
            .ForMember(dest => dest.TrackingNumber, opt => opt.MapFrom(src => src.TrackingNumber.Value))
            .ForMember(dest => dest.OriginPort, opt => opt.MapFrom(src => src.OriginPort.Code))
            .ForMember(dest => dest.DestinationPort, opt => opt.MapFrom(src => src.DestinationPort.Code))
            .ForMember(dest => dest.WeightKg, opt => opt.MapFrom(src => src.WeightKg.Value))
            .ForMember(dest => dest.VolumeCbm, opt => opt.MapFrom(src => src.VolumeCbm.Value))
            .ForMember(dest => dest.StatusHistory, opt => opt.MapFrom(src =>
                src.StatusHistory
                    .OrderBy(history => history.EventTimeUtc)));

        CreateMap<Shipment, PublicTrackingDto>()
            .ForMember(dest => dest.TrackingNumber, opt => opt.MapFrom(src => src.TrackingNumber.Value))
            .ForMember(dest => dest.StatusHistory, opt => opt.MapFrom(src =>
                src.StatusHistory
                    .OrderBy(history => history.EventTimeUtc)));
    }
}
