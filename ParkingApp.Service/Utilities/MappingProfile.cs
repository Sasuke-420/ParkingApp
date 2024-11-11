using AutoMapper;
using Lisec.ParkingApp.DTOs;
using Lisec.ParkingApp.Models;
using Lisec.UserManagementDB.Domain.Models.Master;
using System.Linq;

namespace Lisec.ParkingApp.Utilities
{
    /// <summary>
    /// MappingProfile
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Constructor for mapping profile class.
        /// </summary>
        public MappingProfile()
        {
            AllowNullCollections = true;
            CreateMap<Lisec.ParkingApp.Models.Group, Lisec.ParkingApp.DTOs.GroupDTO>().ForMember(src => src.MemeberIds, dest => dest.Ignore()).AfterMap((src, dest) =>
            {
                dest.Modified = src.Modified.Value.ToUniversalTime();
                dest.MemeberIds = src.MemeberIds.Split(',').Select(x => int.Parse(x)).ToList();
            });
            CreateMap<CardDTO, Card>().AfterMap((src, dest) =>
            {
                dest.Modified = src.Modified.ToUniversalTime();
            }).ReverseMap();
            CreateMap<CreateCardDTO, Card>().AfterMap((src, dest) =>
            {
                dest.Modified = src.Modified.ToUniversalTime();
            }).ReverseMap();

            CreateMap<GroupDTO, Group>().AfterMap((src, dest) =>
            {
                dest.Modified = src.Modified.ToUniversalTime();
                dest.MemeberIds = string.Join(',', src.MemeberIds);
            });


            CreateMap<Group, CreateGroupDTO>().ForMember(src => src.MemeberIds, dest => dest.Ignore()).AfterMap((src, dest) =>
            {
                dest.Modified = src.Modified.Value.ToUniversalTime();
                dest.MemeberIds = src.MemeberIds.Split(',').Select(x => int.Parse(x)).ToList();
            });

            CreateMap<CreateGroupDTO, Group>().ForMember(src => src.MemeberIds, dest => dest.Ignore()).AfterMap((src, dest) =>
            {
                dest.Modified = src.Modified.ToUniversalTime();
                dest.MemeberIds = string.Join(',', src.MemeberIds);
            });

            CreateMap<PaidParkingDTO, PaidParking>().AfterMap((src, dest) =>
            {
                dest.Modified = src.Modified.ToUniversalTime();
                dest.SharesId = string.Join(',', src.Users.Select(x => x.Id));
            });

            CreateMap<PaidParking, PaidParkingDTO>().AfterMap((src, dest) =>
            {
                var users = src.SharesId.Split(',').Select(x => new UserDTO { Id = int.Parse(x) }).ToList();
                dest.Modified = src.Modified.Value.ToUniversalTime();
                dest.Users = users;
            });

            CreateMap<PaidParking, CreatePaidParkingDTO>().ForMember(src => src.SharesId, dest => dest.Ignore()).AfterMap((src, dest) =>
            {
                dest.Modified = src.Modified.Value.ToUniversalTime();
                dest.SharesId = src.SharesId.Split(',').Select(x => int.Parse(x)).ToList();
            });

            CreateMap<CreatePaidParkingDTO, PaidParking>().AfterMap((src, dest) =>
            {
                dest.Modified = src.Modified.ToUniversalTime();
                if (src.SharesId != null)
                    dest.SharesId = string.Join(',', src.SharesId);
            });

            CreateMap<MasterUser, UserDTO>()
                .AfterMap((src, dest) =>
                {
                    dest.Id = src.UserId;
                    dest.Name = src.FirstName + " " + src.LastName;
                    dest.Email = src.Email;
                });

            CreateMap<UserCarDTO, UserCar>().AfterMap((src, dest) =>
            {
                dest.Modified = src.Modified.ToUniversalTime();
            }).ReverseMap();
            CreateMap<CreateUserCarDTO, UserCar>().AfterMap((src, dest) =>
            {
                dest.Modified = src.Modified.ToUniversalTime();
            }).ReverseMap();

            CreateMap<UserCardDTO, UserCard>().AfterMap((src, dest) =>
            {
                dest.Modified = src.Modified.ToUniversalTime();
            }).ReverseMap();
            CreateMap<CreateUserCardDTO, UserCard>().AfterMap((src, dest) =>
            {
                dest.Modified = src.Modified.ToUniversalTime();
            }).ReverseMap();

            CreateMap<RestrictionDTO, Restriction>().ReverseMap();
            CreateMap<UpsertRestrictionDTO, Restriction>().ReverseMap();

            CreateMap<MessageDTO, MessageModel>().ReverseMap();
            CreateMap<CreateMessageDTO, MessageModel>().ReverseMap();
        }
        public static Mapper CreateMapper()
        {
            MappingProfile mp = new MappingProfile();
            var config = new MapperConfiguration(cfg => cfg.AddProfile(mp));
            return new Mapper(config);
        }
    }
}
