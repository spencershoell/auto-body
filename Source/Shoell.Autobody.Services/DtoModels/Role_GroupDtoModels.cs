namespace Shoell.Autobody.Services.Role_GroupDtoModels
{
    public class AddGroupToRolesDto
    {
        public Guid GroupId { get; set; }
        public List<Guid> RoleIds { get; set; } = [];
    }

    public class AddRoleToGroupsDto
    {
        public Guid RoleId { get; set; }
        public List<Guid> GroupIds { get; set; } = [];
    }
}
