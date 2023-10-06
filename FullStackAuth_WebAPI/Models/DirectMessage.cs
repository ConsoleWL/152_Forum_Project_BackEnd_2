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
        public DateTime? MessageTime { get; set; }
        public string UserIdToId { get; set; }
        [ForeignKey("UserIdToId")]
        public User UserIdTo { get; set; }

        public string UserIdFromId { get; set; }
        [ForeignKey("UserIdFromId")]
        public User UserIdFrom { get; set; }
        //nav

       

    }

    public class DirectMessageDateComparer : IComparer<DirectMessage>
    {
        public int Compare(DirectMessage p1, DirectMessage p2)
        {
            if (p1.MessageTime > p2.MessageTime)
                return 1;
            else if (p1.MessageTime < p2.MessageTime)
                return -1;
            else
                return 0;
        }
    }
}

