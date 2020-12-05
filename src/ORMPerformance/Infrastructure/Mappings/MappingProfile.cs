using AutoMapper;
using ORMPerformance.Data.Domain;
using ORMPerformance.Infrastructure.Dtos;
using ORMPerformance.Infrastructure.Dtos.EntityDtos;
using ORMPerformance.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ORMPerformance.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrderDto>();
            CreateMap<Customer, CustomerDto>();
            CreateMap<Card, CardDto>();

            CreateMap<Order, OrderWithRelationsDto>()
                .ForMember(o => o.CustomerDto, opt => opt.MapFrom(o => o.Customer));

            CreateMap<Customer, CustomerWithRelationsDto>()
                .ForMember(o => o.OrdersDto, opt => opt.MapFrom(o => o.Orders));
                //.ForMember(o => o.CardsDto, opt => opt.MapFrom(o => o.Cards))
        }
    }
}
