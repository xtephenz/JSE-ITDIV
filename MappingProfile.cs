using AutoMapper;
using JSE.Models;
using JSE.Models.Requests;
using JSE.Models.Results;

namespace JSE
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Delivery, GetDeliveryResult>()
                .ForMember(dest => dest.SenderPool, opt => opt.MapFrom(src => new GetPoolResult()
                {
                    pool_name = src.SenderPool.pool_name,
                    pool_phone = src.SenderPool.pool_phone,
                }))
                .ForMember(dest => dest.ReceiverPool, opt => opt.MapFrom(src => new GetPoolResult()
                {
                    pool_name = src.ReceiverPool.pool_name,
                    pool_phone = src.ReceiverPool.pool_phone,
                }))
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages))
                .ForMember(dest => dest.Courier, opt => opt.MapFrom(src => new GetCourierResult()
                {
                    courier_username = src.Courier.courier_username,
                    courier_phone = src.Courier.courier_phone,
                }));

            CreateMap<PoolBranch, GetPoolResult>();
            CreateMap<Courier, GetCourierResult>();
            CreateMap<Message, GetMessageResult>();

            CreateProjection<Delivery, GetDeliveryResult>();
            CreateMap<Delivery, GetDeliveryListCourier>();

            CreateMap<CreateDelivery, Delivery>();
            CreateMap<GetMessageResult, Message>();

            //AssertConfigurationIsValid();
        }
        
    }
}
