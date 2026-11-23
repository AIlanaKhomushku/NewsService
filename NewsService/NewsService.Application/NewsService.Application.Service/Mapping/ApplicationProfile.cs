using AutoMapper;
using NewsService.Application.Models.Author;
using NewsService.Application.Models.Comment;
using NewsService.Application.Models.News;
using NewsService.Application.Models.Reaction;
using NewsService.Application.Models.User;
using NewsService.Domain.NewsService.Domain;

namespace NewsService.Application.Services.Mapping;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        CreateMap<Author, AuthorModel>()
            .ForMember(dest => dest.Authorname, opt => opt.MapFrom(src => src.Authorname.Value));

        CreateMap<User, UserModel>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username.Value));

        CreateMap<News, NewsModel>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title.Value))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content.Value))
            .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.Author.Id));

        CreateMap<Comment, CommentModel>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content.Value))
            .ForMember(dest => dest.NewsId, opt => opt.MapFrom(src => src.News.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id));

        CreateMap<Reaction, ReactionModel>()
            .ForMember(dest => dest.NewsId, opt => opt.MapFrom(src => src.News.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id));
    }
}