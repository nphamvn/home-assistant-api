using System.Collections.Generic;
using System.Threading.Tasks;
using HomeAssistant.API.Data;
using HomeAssistant.API.Entities;
using HomeAssistant.API.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace HomeAssistant.API.Tests;

public class Tests
{
    ApplicationDbContext _context;

    [SetUp]
    public async Task Setup()
    {
        var option = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(databaseName: "Test_Database")
                            .Options;
        _context = new ApplicationDbContext(option);
        if (_context != null)
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }
        var user1 = new AppUser()
        {
            UserName = "user1",
            Email = "user1@email.com",
        };
        var user2 = new AppUser()
        {
            UserName = "user2",
            Email = "user2@email.com",
        };
        await _context.Users.AddRangeAsync(user1, user2);
        await _context.SaveChangesAsync();

        var conversation = new Conversation()
        {
            Creator = user1,
            Description = "Test Conversation",
        };

        conversation.UserConversations = new List<UserConversation>()
        {
            new UserConversation()
            {
                User = user1,
                Conversation = conversation,
            },
            new UserConversation()
            {
                User = user2,
                Conversation = conversation,
            },
        };

        await _context.Conversations.AddAsync(conversation);
        await _context.SaveChangesAsync();
    }

    [Test]
    public async Task Test1()
    {
        int count = await _context.Users.CountAsync();
        Assert.AreEqual(2, count);

        count = await _context.Conversations.CountAsync();
        Assert.AreEqual(1, count);

        //var uc = await _context.UserConversations.CountAsync();
        //Assert.AreEqual(2, uc);
    }
}