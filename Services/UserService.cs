using MongoDB.Driver;
using UserLoginApp.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace UserLoginApp.Services;

public class UserService
{
  private readonly IMongoCollection<User> _userCollection;
  public UserService(IOptions<MongoDBSettings> mongoSettings)
  {
    var mongoClient = new MongoClient(mongoSettings.Value.ConnectionURI);
    var mongoDatabase = mongoClient.GetDatabase(mongoSettings.Value.DatabaseName);
    _userCollection = mongoDatabase.GetCollection<User>(mongoSettings.Value.UserCollectionName);
  }

  public List<User> GetAllUsers() => _userCollection.Find(_ => true).ToList();

  public void CreateUser(User newUser) => _userCollection.InsertOne(newUser);

  public User GetUserById(string id)
  {
    return _userCollection.Find(user => user.Id == id).FirstOrDefault();
  }
  public void UpdateUser(string id, User updatedUser)
  {
    updatedUser.Id = id; // ensure ID stays consistent
    _userCollection.ReplaceOne(user => user.Id == id, updatedUser);
  }

public void DeleteUser(string id)
{
    _userCollection.DeleteOne(user => user.Id == id);
}

 }

  