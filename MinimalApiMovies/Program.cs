using Microsoft.AspNetCore.Cors;
using MinimalApiMovies.Entities;

var builder = WebApplication.CreateBuilder(args);

#region =========Services=========

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder => policyBuilder.WithOrigins(builder.Configuration["AllowedCORSOrigins"] ?? string.Empty) .AllowAnyMethod().AllowAnyHeader());
    options.AddPolicy("AllowAllOrigins", policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
builder.Services.AddOutputCache();

#endregion

var app = builder.Build();

#region =========Pipeline && Middleware=========

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseCors();
app.UseOutputCache();

app.MapGet("/", () => "Hello Taosif");
app.MapGet("/genres", [EnableCors(PolicyName = "AllowAllOrigins")] () =>
{
    var genres = new List<Genre>
    {
        new() { Id = 1, Name = "Action" },
        new() { Id = 2, Name = "Comedy" },
        new() { Id = 3, Name = "Drama" },
        new() { Id = 4, Name = "Sci-Fi" }
    };
    return genres;
}).CacheOutput(x=> x.Expire(TimeSpan.FromSeconds(60)));
#endregion

app.Run();