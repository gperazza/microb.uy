namespace MicrobUy_API.Dtos.SuggestNeo4jDto
{
    public class SenttingSuggestUsersNeo4jDto
    {
        //an attribute in the class, for each relationship with importance in Neo4j
        public int POSTED {  get; set; }
        public int LIVE { get; set; }
        public int LIKE { get; set; }
        public int HAVE { get; set; }
        public int BORN { get; set; }
        public int WITH_HASHTAG { get; set; }

    }
}
