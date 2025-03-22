using Shoell.Autobody.Models.Identity;

namespace Shoell.Autobody.Data.Tests
{
    public static class UserExtensions
    {
        public static User PopulateDefaultValues(this User user)
        {
            user.Id = Guid.NewGuid();
            user.ExternalId = Guid.NewGuid().ToString();
            user.Name = user.Id.ToString();
            user.UserName = user.Id.ToString();
            user.NormalizedUserName = user.UserName.ToUpper();

            return user;
        }
    }
}
