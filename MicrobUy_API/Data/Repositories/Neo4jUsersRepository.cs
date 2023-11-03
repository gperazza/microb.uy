﻿using MicrobUy_API.Models;
using MicrobUy_API.Dtos;
using Neo4j.Driver;
using static MicrobUy_API.Models.UserModel;
using System;

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
        //returns a IEnumerable of UserSuggestDto
        private static IEnumerable<UserSuggestDto> MapCastUser(IEnumerable<IDictionary<string, object>> users)
        {
            return users
                .Select(dictionary =>
                    new UserSuggestDto(
                        dictionary["name"].As<string>()
                    )
                ).ToList();
        }
        //Create a node in Neo4j user, city and occupation, along with their relationship
        public async Task<int> CreateUser(int userId, int tenantId, string username, string occupation, string city)
        {
            await using var session = _driver.AsyncSession(WithDatabase);

            //Falta comprobar que el usuario exista con userId, no crearlo

            return await session.ExecuteWriteAsync(async transaction =>
            {
                var cursor = await transaction.RunAsync(@"CREATE (user:User { name: $username, tenantID: $tenantId, userId: $userId }) 
                                                          CREATE (city:City {name: $city})
                                                          CREATE (ocupation:Ocupation {name: $occupation})
                                                          CREATE (user)-[r1:LIVE{importance:10}]->(city),
                                                          (user)-[r2:HAVE{importance:20}]->(ocupation)",
                                                          new { userId, tenantId, username, occupation, city });

                var summary = await cursor.ConsumeAsync();
                return summary.Counters.NodesCreated + summary.Counters.RelationshipsCreated;
            });
        }
        //Create a node in Neo4j Post and relate it to the user who created said post
        public async Task<int> CreatePost(int userId, int tenantId, int postId, string postCreated)
        {
            await using var session = _driver.AsyncSession(WithDatabase);

            //Falta comprobar que el post exista con postId, no crearlo

            return await session.ExecuteWriteAsync(async transaction =>
            {
                var cursor = await transaction.RunAsync(@"MATCH (u:User {userId: $userId}) 
                                                          CREATE (pos:Post {postId: $postId, tenantId: $tenantId, postCreated:date($postCreated)})
                                                          CREATE  (u)-[r1:POSTED]->(pos)",
                                                          new { userId, tenantId, postId, postCreated });

                var summary = await cursor.ConsumeAsync();
                return summary.Counters.NodesCreated + summary.Counters.RelationshipsCreated;
            });
        }
        //Update user data
        public async Task<int> UpdateUser(int userId, int tenantId, string username, string occupation, string city)
        {
            await using var session = _driver.AsyncSession(WithDatabase);

            //Falta comprobar que el usuario exista con userId, tirar exepcion

            return await session.ExecuteWriteAsync(async transaction =>
            {
                var cursor = await transaction.RunAsync(@"MATCH (u:User)
                                                          WHERE u.userId = $userId AND u.tenantID = $tenantId
                                                          SET u.name = $username",
                                                          new { userId, tenantId, username, occupation, city });

                var summary = await cursor.ConsumeAsync();
                return summary.Counters.NodesCreated + summary.Counters.RelationshipsCreated;
            });
        }
    }
}
