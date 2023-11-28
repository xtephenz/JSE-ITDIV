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
            CreateMap<Delivery, GetDeliveryResult>().ForMember(
                dest => dest.SenderPool, opt => opt.MapFrom(src => src.SenderPool)
            ).ForMember(
                dest => dest.ReceiverPool, opt => opt.MapFrom(src => src.ReceiverPool)
            ).ForMember(
                dest => dest.Messages, opt => opt.MapFrom(src => src.Messages));
            CreateMap<PoolBranch, GetPoolResult>();
            CreateMap<Message, GetMessageResult>();

            CreateMap<CreateDelivery, Delivery>();
            CreateMap<GetMessageResult, Message>();
        }
    }
}
