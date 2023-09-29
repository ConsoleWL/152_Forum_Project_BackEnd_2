using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace FullStackAuth_WebAPI.Models
{
    public class DirectMessage
    {
        [Key]
        public int DirectMessageId { get; set; }

        [MaxLength(200, ErrorMessage = "200 symbols max")]
        public string Text { get; set; }

        public string UserIdTo { get; set; }
        public string UserIdFrom { get; set; }
        //nav
        public List<User> Users { get; set; }

    }
}
