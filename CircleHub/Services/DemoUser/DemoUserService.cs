using Bogus;
using CircleHub.Data;
using CircleHub.Models;
using CircleHub.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CircleHub.Services.DemoUser;

public class DemoUserService(UserManager<ApplicationUser> userManager, IDbContextFactory<ApplicationDbContext> contextFactory) : IDemoUserService
{
    public async Task<ApplicationUser> CreateDemoUserAsync()
    {
        var demoId = Guid.NewGuid().ToString("N");

        var user = new ApplicationUser
        {
            UserName = $"demo-{demoId}@demo.local",
            Email = $"demo-{demoId}@demo.local",
            FirstName = "Demo",
            LastName = "User",
            IsDemo = true,
            EmailConfirmed = true,
            DemoLastActivity = DateTime.UtcNow
        };

        var password = $"Demo{Guid.NewGuid():N}!aB1";

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));

            throw new Exception($"Failed to create demo user: {errors}");
        }

        await using var context = await contextFactory.CreateDbContextAsync();

        await CreateDemoDataAsync(user, context);
        return user;
    }

    private static async Task CreateDemoDataAsync(ApplicationUser user, ApplicationDbContext context)
    {
        var demoContacts = await context.Contacts
            .Where(c => c.AppUserId == user.Id)
            .Include(c => c.Categories)
            .ToListAsync();

        var demoCategories = await context.Categories
            .Where(c => c.AppUserId == user.Id)
            .ToListAsync();

        Random rand = new();

        if (demoContacts.Count == 0)
        {
            var newContacts = new Faker<Contact>()
                .RuleFor(c => c.LastName, f => f.Name.LastName())
                .RuleFor(c => c.BirthDate, f => DateOnly.FromDateTime(f.Date.Between(
                    DateTime.Today.AddYears(-60),
                    DateTime.Today.AddYears(-18)
                    )))
                .RuleFor(c => c.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.Address1, f => f.Address.StreetAddress())
                .RuleFor(c => c.City, f => f.Address.City())
                .RuleFor(c => c.PostalCode, f => f.Address.ZipCode())
                .RuleFor(c => c.AppUserId, user.Id)
                .Generate(10);

            Faker faker = new();

            var imageDir = Path.Combine(Directory.GetCurrentDirectory(), "Data/DemoImages");
            var mensPics = Directory.GetFiles(Path.Combine(imageDir, "Men/")).ToList();
            var womensPics = Directory.GetFiles(Path.Combine(imageDir, "Women/")).ToList();

            for (int i = 0; i < newContacts.Count; i++)
            {
                Contact contact = newContacts[i];

                if (i % 2 == 0)
                {
                    contact.FirstName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male);
                    if (mensPics.Count > 0)
                    {
                        var pic = mensPics[rand.Next(0, mensPics.Count)];
                        mensPics.Remove(pic);

                        ImageUpload img = new()
                        {
                            Data = await File.ReadAllBytesAsync(pic),
                            Type = $"image/{Path.GetExtension(pic).TrimStart('.')}"
                        };

                        contact.Image = img;
                        context.Images.Add(img);
                    }
                }
                else
                {
                    contact.FirstName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female);
                    if (womensPics.Count > 0)
                    {
                        var pic = womensPics[rand.Next(0, womensPics.Count)];
                        womensPics.Remove(pic);

                        ImageUpload img = new()
                        {
                            Data = await File.ReadAllBytesAsync(pic),
                            Type = $"image/{Path.GetExtension(pic).TrimStart('.')}"
                        };

                        contact.Image = img;
                        context.Images.Add(img);
                    }
                }

                contact.Email = faker.Internet.Email(contact.FirstName, contact.LastName, "mailinator.com");
                if (rand.Next() % 2 == 0)
                {
                    contact.Address2 = new Faker().Address.SecondaryAddress();
                }
            }

            demoContacts.AddRange(newContacts);
        }

        if (demoCategories.Count == 0)
        {
            demoCategories = [
                new() { Name = "Family", AppUserId = user.Id },
                new() { Name = "Friends", AppUserId = user.Id },
                new() { Name = "Coworkers", AppUserId = user.Id },
                new() { Name = "Clients", AppUserId = user.Id },
                new() { Name = "Gaming", AppUserId = user.Id },
                new() { Name = "Favorites", AppUserId = user.Id }
                ];
            context.Categories.AddRange(demoCategories);
        }

        foreach (var contact in demoContacts.Where(c => c.Categories.Count == 0))
        {
            int numCategories = rand.Next(1, 5);
            var categories = demoCategories
                .OrderBy(c => Guid.NewGuid())
                .Take(numCategories);

            contact.Categories = [.. categories];
            context.Update(contact);
        }

        await context.SaveChangesAsync();

    }
}
