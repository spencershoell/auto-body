namespace Shoell.Autobody.Services.User_GroupDtoModels
{
    public class AddGroupToUsersDto
    {
        public Guid GroupId { get; set; }
        public List<Guid> UserIds { get; set; } = [];
    }

    public class AddUserToGroupsDto
    {
        public Guid UserId { get; set; }
        public List<Guid> GroupIds { get; set; } = [];
    }
}
