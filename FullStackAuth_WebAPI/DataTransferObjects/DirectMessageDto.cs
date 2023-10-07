using FullStackAuth_WebAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FullStackAuth_WebAPI.DataTransferObjects
{
    public class DirectMessageDto
    {
 
        public int DirectMessageId { get; set; }
 
        public string Text { get; set; }
        public DateTime? MessageTime { get; set; }
        public string UserIdToId { get; set; }

        public string UserIdToName { get; set; }

        public string UserIdFromId { get; set; }
   
        public string UserIdFromName { get; set; }
       
    }
}
