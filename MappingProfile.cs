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
            CreateMap<PoolBranch, GetPoolResult>();
            CreateMap<Message, GetMessageResult>();
            CreateMap<Courier, GetCourierResult>();
            CreateMap<Delivery, GetDeliveryResult>();
                //.ForMember(dest => dest.SenderPool, opt => opt.MapFrom(src => src.SenderPool))
                //.ForMember(dest => dest.ReceiverPool, opt => opt.MapFrom(src => src.ReceiverPool))
                //.ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages))
                //.ForMember(dest => dest.Courier, opt => opt.MapFrom(src => src.Courier));


            CreateMap<Delivery, GetDeliveryListCourier>();

            CreateMap<CreateDelivery, Delivery>();
            CreateMap<GetMessageResult, Message>();
        }
        private GetCourierResult MapCourier(Courier courier)
        {
            if (courier != null)
            {
                return new GetCourierResult
                {
                    courier_username = courier.courier_username,
                    courier_phone = courier.courier_phone
                };
            }

            return null;
        }
    }
}
