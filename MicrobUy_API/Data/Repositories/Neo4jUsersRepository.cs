using MicrobUy_API.Models;
using MicrobUy_API.Dtos;
using Neo4j.Driver;
using static MicrobUy_API.Models.UserModel;
using System;
using MicrobUy_API.Dtos.SuggestNeo4jDto;

namespace MicrobUy_API.Data.Repositories
{
    public class Neo4jUsersRepository : INeo4jUsersRepository
    {
        private IDriver _driver;
        public Neo4jUsersRepository(IDriver driver)
        {
            _driver = driver;
        }
        private static void WithDatabase(SessionConfigBuilder sessionConfigBuilder)
        {
            var neo4jVersion = Environment.GetEnvironmentVariable("NEO4J_VERSION") ?? "";
            if (!neo4jVersion.StartsWith("4"))
                return;

            sessionConfigBuilder.WithDatabase(Database());
        }
        //Get a name of DB
        private static string Database()
        {
            return Environment.GetEnvironmentVariable("NEO4J_DATABASE") ?? "node4j";
        }
        //Create a node in Neo4j user, city and occupation, along with their relationship
        public async Task CreateUser(CreateUserNeo4jDto createUsNeo4jDto)
        {
            await using var session = _driver.AsyncSession(WithDatabase);

             await session.ExecuteWriteAsync(async transaction =>
            {
                var cursor = await transaction.RunAsync(@"
                    MERGE (user:User { name: $UserName, tenantID: $TenantId, userId: $UserId, isSanctioned: $IsSanctioned, creationDate:date($CreationDate)}) 
                        ON CREATE SET user.userId = $UserId
                    MERGE (birth:Birthday {birthdayDate:date($Birthday)})
                    MERGE (city:City {name: $City, tenantID: $TenantId})
                    MERGE (ocupation:Ocupation {name: $Occupation, tenantID: $TenantId})
                    CREATE (user)-[:LIVE{importance:10}]->(city),
                           (user)-[:BORN{importance:30}]->(birth),
                           (user)-[:HAVE{importance:20}]->(ocupation)",
                    new { createUsNeo4jDto.UserId, createUsNeo4jDto.TenantId, createUsNeo4jDto.UserName, createUsNeo4jDto.Occupation, createUsNeo4jDto.City, createUsNeo4jDto.CreationDate, createUsNeo4jDto.IsSanctioned, createUsNeo4jDto.Birthday });

                var summary = await cursor.ConsumeAsync();
            });
        }
        //Create a node in Neo4j Post and relate it to the user who created said post
        public async Task<int> CreatePost(CrearPostNeo4jDto crearPDto)
        {
            await using var session = _driver.AsyncSession(WithDatabase);
            return await session.ExecuteWriteAsync(async transaction =>
            {
                var cursor = await transaction.RunAsync(@"
                    MATCH (u:User {userId: $userId}) 
                    MERGE (pos:Post {postId: $postId, tenantId: $tenantId, postCreated:date($postCreated), isSanctioned: $isSanctioned})
                    CREATE (u)-[r2:POSTED]->(pos)
                    WITH pos
                    UNWIND $hashtags AS hashtag
                    MERGE (h:Hashtag {name: hashtag, tenantID: $tenantId})
                    CREATE (pos)-[r1:WITH_HASHTAG{importance:40}]->(h)",
                    new { crearPDto.userId, crearPDto.tenantId, crearPDto.postId, crearPDto.postCreated, crearPDto.isSanctioned, crearPDto.hashtags });

                var summary = await cursor.ConsumeAsync();
                return summary.Counters.NodesCreated + summary.Counters.RelationshipsCreated + summary.Counters.PropertiesSet + summary.Counters.LabelsAdded;
            });
        }
        //Update user data
        public async Task<int> UpdateUser(CreateUserNeo4jDto createUsNeo4jDto)
        {
            await using var session = _driver.AsyncSession(WithDatabase);

            return await session.ExecuteWriteAsync(async transaction =>
            {
                var cursor = await transaction.RunAsync(@"
                    MATCH (u:User)
                    WHERE u.userId = $UserId AND u.tenantID = $TenantId SET u.name = $UserName
                    MERGE (city:City {name: $City, tenantID: $TenantId})
                    MERGE (ocupation:Ocupation {name: $Occupation, tenantID: $TenantId})
                    MERGE (birth:Birthday {birthdayDate:date($Birthday)})
                    MERGE (u)-[:LIVE {importance: 10}]->(city)
                    MERGE (u)-[:HAVE {importance: 20}]->(ocupation)
                    MERGE (u)-[:BORN{importance:30}]->(birth)",
                    new { createUsNeo4jDto.UserId, createUsNeo4jDto.TenantId, createUsNeo4jDto.UserName, createUsNeo4jDto.Occupation, createUsNeo4jDto.City, createUsNeo4jDto.CreationDate, createUsNeo4jDto.IsSanctioned, createUsNeo4jDto.Birthday });

                var summary = await cursor.ConsumeAsync();
                return summary.Counters.NodesCreated + summary.Counters.RelationshipsCreated + summary.Counters.PropertiesSet + summary.Counters.LabelsAdded;
            });
        }
        //Created a relationship whit user and post whit liked
        public async Task<int> GiveLike(GiveLikeNeo4jDto giveLikeNeo4JDto)
        {
            await using var session = _driver.AsyncSession(WithDatabase);
            return await session.ExecuteWriteAsync(async transaction =>
            {
                var cursor = await transaction.RunAsync(@"
                    MATCH (u:User) 
                    WHERE u.userId = $UserId AND u.tenantID = $TenantId
                    MATCH (pos:Post) WHERE pos.postId = $PostId
                    MERGE (u)-[:LIKE{importance:50}]->(pos)",
                    new { giveLikeNeo4JDto.UserId, giveLikeNeo4JDto.TenantId, giveLikeNeo4JDto.PostId });
                var summary = await cursor.ConsumeAsync();
                return summary.Counters.NodesCreated + summary.Counters.RelationshipsCreated + summary.Counters.PropertiesSet + summary.Counters.LabelsAdded;
            });
        }
        //Borra una relacion de like
        public async Task<int> DeleteLike(GiveLikeNeo4jDto giveLikeNeo4JDto)
        {
            await using var session = _driver.AsyncSession(WithDatabase);
            return await session.ExecuteWriteAsync(async transaction =>
            {
                var cursor = await transaction.RunAsync(@"
                    MATCH (u:User) 
                    WHERE u.userId = $UserId AND u.tenantID = $TenantId
                    MATCH (pos:Post) WHERE pos.postId = $PostId
                    WITH pos, u
                    MATCH (u)-[r:LIKE{importance:50}]->(pos)
                    delete r",
                    new { giveLikeNeo4JDto.UserId, giveLikeNeo4JDto.TenantId, giveLikeNeo4JDto.PostId });
                var summary = await cursor.ConsumeAsync();
                return summary.Counters.RelationshipsDeleted;
            });
        }
        //top hashtags de un tenant en particular
        public async Task<List<HashtagNeo4jDto>> TopHashtagByTenant(int tenantId, int topCant)
        {
            await using var session = _driver.AsyncSession(WithDatabase);
            return await session.ExecuteReadAsync(async transaction =>
            {
                var result = await transaction.RunAsync(@"
                    MATCH (h:Hashtag) where h.tenantID = $tenantId
                    WITH h, [()-[:WITH_HASHTAG]->(h) |1] AS relationships
                    WHERE SIZE(relationships) > 0
                    WITH h, SIZE(relationships) AS relationshipCount
                    ORDER BY relationshipCount DESC
                    limit $topCant
                    RETURN distinct h.name AS Name, relationshipCount",//COLLECT(pos.postId) AS PostIds
                    new { tenantId, topCant });
                List<HashtagNeo4jDto> hashtags = await result.ToListAsync(record =>
                {
                    return new HashtagNeo4jDto
                    {
                        Name = record["Name"].As<string>(),
                        TenantID = tenantId,
                        RelationshipCount = record["relationshipCount"].As<int>()
                    };
                });
                return hashtags;
            });
        }
        //top hashtags de toda la paltaforma
        public async Task<List<HashtagNeo4jDto>> TopHashtagAllTenant(int topCant)
        {
            await using var session = _driver.AsyncSession(WithDatabase);
            return await session.ExecuteReadAsync(async transaction =>
            {
                var result = await transaction.RunAsync(@"
                    MATCH (h:Hashtag)
                    WITH h, [()-[:WITH_HASHTAG]->(h) | 1] AS relationships
                    WHERE SIZE(relationships) > 0
                    WITH h, SIZE(relationships) AS relationshipCount
                    ORDER BY relationshipCount DESC
                    LIMIT $topCant
                    RETURN h.name AS Name, h.tenantID AS TenantId, relationshipCount",
                    new {topCant });
                List<HashtagNeo4jDto> hashtags = await result.ToListAsync(record =>
                {
                    return new HashtagNeo4jDto
                    {
                        Name = record["Name"].As<string>(),
                        TenantID = record["TenantId"].As<int>(),
                        RelationshipCount = record["relationshipCount"].As<int>()
                    };
                });
                return hashtags;
            });
        }
        //Obtiene los post con mas like en todas las instancias
        public async Task<List<PostWhitMostLikeNeo4jDto>> PostWhitMostLikeAllTenant(int topCant)
        {
            await using var session = _driver.AsyncSession(WithDatabase);
            return await session.ExecuteReadAsync(async transaction =>
            {
                var result = await transaction.RunAsync(@"
                    MATCH (p:Post)
                    WITH p, [()-[:LIKE]->(p) |1] AS relationships
                    WHERE SIZE(relationships) > 0
                    WITH p, SIZE(relationships) AS relationshipCount
                    ORDER BY relationshipCount DESC
                    limit $topCant
                    RETURN p.postId AS PostId,p.tenantId AS TenantId , relationshipCount",
                    new { topCant });
                List<PostWhitMostLikeNeo4jDto> posts = await result.ToListAsync(record =>
                {
                    return new PostWhitMostLikeNeo4jDto
                    {
                        PostId = record["PostId"].As<int>(),
                        TenantId = record["TenantId"].As<int>(),
                        RelationshipCount = record["relationshipCount"].As<int>()
                    };
                });
                return posts;
            });

        }
        //Obtiene los post con mas like en una instancia especificada
        public async Task<List<PostWhitMostLikeNeo4jDto>> PostWhitMostLikeByTenant(int tenantId, int topCant)
        {
            await using var session = _driver.AsyncSession(WithDatabase);
            return await session.ExecuteReadAsync(async transaction =>
            {
                var result = await transaction.RunAsync(@"
                    MATCH (p:Post) where p.tenantId = $tenantId
                    WITH p, [()-[:LIKE]->(p) |1] AS relationships
                    WHERE SIZE(relationships) > 0
                    WITH p, SIZE(relationships) AS relationshipCount
                    ORDER BY relationshipCount DESC
                    limit $topCant
                    RETURN p.postId AS PostId, relationshipCount",
                    new { tenantId, topCant });
                List<PostWhitMostLikeNeo4jDto> posts = await result.ToListAsync(record =>
                {
                    return new PostWhitMostLikeNeo4jDto
                    {
                        PostId = record["PostId"].As<int>(),
                        TenantId = tenantId,
                        RelationshipCount = record["relationshipCount"].As<int>()
                    };
                });
                return posts;
            });
        }
        //Funcion para recomendar usuarios, segun si tiene nodos en comun y la sumatoria del atributo importance
        public async Task<List<SuggestUserNeo4jDto>> SuggestUsersByTenant(int tenantId, int userId, int topCant)
        {
            await using var session = _driver.AsyncSession(WithDatabase);
            return await session.ExecuteReadAsync(async transaction =>
            {
                var result = await transaction.RunAsync(@"
                    MATCH (u:User {userId: $userId, tenantID: $tenantId})
                    MATCH (similarUser:User)-[r]->(commonNode)
                    WHERE similarUser.tenantID = $tenantId
                      AND u <> similarUser
                      AND r.importance > 0
                      AND NOT similarUser.isSanctioned
                    WITH similarUser, SUM(r.importance) AS totalImportance
                    ORDER BY totalImportance DESC
                    LIMIT $topCant
                    RETURN similarUser.tenantID AS TenantId, similarUser.userName AS UserName, totalImportance
                    ",
                    new { tenantId, topCant, userId });
                List<SuggestUserNeo4jDto> posts = await result.ToListAsync(record =>
                {
                    return new SuggestUserNeo4jDto
                    {
                        UserName = record["UserName"].As<string>(),
                        TenantId = record["TenantId"].As<int>(),
                        totalImportance = record["totalImportance"].As<int>()
                    };
                });
                return posts;
            });
        }
        //Permite ajustar la importanca que tiene cada realcion
        public async Task SenttingSuggestUsersAllTenant(SenttingSuggestUsersNeo4jDto senttingSuggestUsersNeo)
        {
            await using var session = _driver.AsyncSession(WithDatabase);
            await session.ExecuteWriteAsync(async transaction =>
            {
                var resultLIVE = await transaction.RunAsync(@"
                    MATCH (User)-[Rl:LIVE]->(City) SET Rl.importance = $LIVE
                    ", new {  senttingSuggestUsersNeo.LIVE});
                await resultLIVE.ConsumeAsync();
                var resultHAVE = await transaction.RunAsync(@"
                    MATCH (User)-[Rh:HAVE]->(Ocupation) SET Rh.importance = $HAVE
                    ", new { senttingSuggestUsersNeo.HAVE });
                await resultHAVE.ConsumeAsync();
                var resultBORN = await transaction.RunAsync(@"
                    MATCH (User)-[Rb:BORN]->(Birthday) SET Rb.importance = $BORN
                    ", new { senttingSuggestUsersNeo.BORN });
                await resultBORN.ConsumeAsync();
                var resultWITH_HASHTAG = await transaction.RunAsync(@"
                    MATCH (Post)-[Rw_h:WITH_HASHTAG]->(Hashtag) SET Rw_h.importance = $WITH_HASHTAG
                    ", new { senttingSuggestUsersNeo.WITH_HASHTAG });
                await resultWITH_HASHTAG.ConsumeAsync();
                var resultLIKE = await transaction.RunAsync(@"
                    MATCH (Post)-[Rlik:LIKE]->(City) SET Rlik.importance = $LIKE
                    ", new { senttingSuggestUsersNeo.LIKE });
                await resultLIKE.ConsumeAsync();
                var resultPOSTED = await transaction.RunAsync(@"
                    MATCH (User)-[Rp:POSTED]->(Post) SET Rp.importance = $POSTED
                    ", new { senttingSuggestUsersNeo.POSTED });
                await resultPOSTED.ConsumeAsync();
            });
        }
    }
}
