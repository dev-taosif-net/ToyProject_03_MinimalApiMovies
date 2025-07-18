using MinimalApiMovies.Entities;

var builder = WebApplication.CreateBuilder(args);

#region =========Services=========

builder.Services.AddOpenApi();

#endregion

var app = builder.Build();

#region =========Pipeline && Middleware=========

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello Taosif");
app.MapGet("/genres", () =>
{
    var genres = new List<Genre>
    {
        new() { Id = 1, Name = "Action" },
        new() { Id = 2, Name = "Comedy" },
        new() { Id = 3, Name = "Drama" },
        new() { Id = 4, Name = "Sci-Fi" }
    };
    return genres;
});

#endregion

app.Run();