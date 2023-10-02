using Microsoft.AspNetCore.Identity;

namespace FullStackAuth_WebAPI.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int? Likes { get; set; } = 0;
        public DateTime? RegistrationDate { get; set; }
        public string ProfilePicture { get; set; }

        //Nav props

        public List<Topic> Topics { get; set; }
        public List<Comment> Comments{ get; set; }
        public List<DirectMessage> DirectMessagesFrom { get; set; }
        public List<DirectMessage> DirectMessagesTo { get; set; }
    }
}

