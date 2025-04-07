using AutoMapper;
using DigitalTherapyBackendApp.Application.Dtos.Subscription;
using DigitalTherapyBackendApp.Domain.Entities.Subscriptions;

namespace DigitalTherapyBackendApp.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Subscription, SubscriptionDto>().ReverseMap();
            CreateMap<UserSubscription, UserSubscriptionDto>().ReverseMap();
            CreateMap<Payment, PaymentDto>().ReverseMap();
            CreateMap<SubscriptionPrice, SubscriptionPriceDto>().ReverseMap();
            CreateMap<SubscriptionTranslation, SubscriptionTranslationDto>().ReverseMap();
        }
    }
}