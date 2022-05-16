namespace SampleMinimal.API.MapProfiles
{
    public class TodoProfile : Profile
    {
        public TodoProfile()
        {
            CreateMap<Todo, TodoDTO>().ReverseMap();
        }
    }
}
