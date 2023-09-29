namespace FullStackAuth_WebAPI.DataTransferObjects
{
    public class CommentsForDisplayDto
    {
        public int CommentId { get; set; }
        public string Text { get; set; }
        public DateTime? TimePosted { get; set; }
        public int? Likes { get; set; }

        public UserNameDto User { get; set; }

    }
}
                       