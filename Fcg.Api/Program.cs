using Fcg.Api.Middlewares;
using Fcg.Application.Requests;
using Fcg.Application.Responses;
using Fcg.Application.Services;
using Fcg.Domain.Entities;
using Fcg.Domain.Queries;
using Fcg.Domain.Repositories;
using Fcg.Domain.Services;
using Fcg.Infrastructure.Data;
using Fcg.Infrastructure.Queries;
using Fcg.Infrastructure.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Prometheus;




var builder = WebApplication.CreateBuilder(args);



#region Services Configuration

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


#region Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT com prefixo 'Bearer '"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
#endregion

#region Database
builder.Services.AddDbContext<FcgDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region MediatR
builder.Services.AddMediatR(fcg =>
    fcg.RegisterServicesFromAssemblyContaining<CreateUserRequest>()
       .RegisterServicesFromAssemblyContaining<CreateAdminUserRequest>());
#endregion

#region Authentication & Authorization
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});
#endregion

#region Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
#endregion

#region Queries
builder.Services.AddScoped<IUserQuery, UserQuery>();
builder.Services.AddScoped<IGameQuery, GameQuery>();
builder.Services.AddScoped<IPromotionQuery, PromotionQuery>();
#endregion

#region Domain Services
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services
    .AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>()
    .AddValidatorsFromAssemblyContaining<CreateAdminUserRequestValidator>();
#endregion

#endregion

var app = builder.Build();

#region Database Migration
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FcgDbContext>();
    dbContext.Database.Migrate();
}
#endregion


#region Minimal APIs



#region Index Endpoint

app.MapGet("/", () => "API rodando no Docker 🚀");

#endregion Index Endpoint


#region User Endpoints
app.MapPost("/api/users", async (CreateUserRequest request, IValidator<CreateUserRequest> validator, IMediator _mediator) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors
            .Select(e => new { e.PropertyName, e.ErrorMessage }));
    }

    var response = await _mediator.Send(request);

    if (response is null || !response.Success)
    {
        return Results.BadRequest(new { Message = response?.Message ?? "Ocorreu um erro ao processar a solicitação." });
    }

    return Results.Created($"/api/users/{response.UserId}", response);
}).AllowAnonymous().WithTags("Users");

app.MapPost("/api/admin/users", async (CreateAdminUserRequest request, IValidator<CreateAdminUserRequest> validator, IMediator _mediator) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors
            .Select(e => new { e.PropertyName, e.ErrorMessage }));
    }

    var response = await _mediator.Send(request) as CreateAdminUserResponse;

    if (response is null || !response.Success)
    {
        return Results.BadRequest(new { Message = response?.Message ?? "Ocorreu um erro ao processar a solicitação." });
    }

    return Results.Created($"/api/users/{response.UserId}", response);
}).WithTags("Users");

app.MapGet("/api/users/{id}", async (Guid id, IUserQuery _userQuery) =>
{
    var user = await _userQuery.GetByIdUserAsync(id);

    return user is not null ? Results.Ok(user) : Results.NotFound();
}).RequireAuthorization().WithTags("Users");

app.MapGet("/api/users/{id}/games", async (Guid id, IUserQuery _userQuery) =>
{
    var user = await _userQuery.GetLibraryByUserAsync(id);

    return user is not null ? Results.Ok(user) : Results.NotFound();
}).RequireAuthorization().WithTags("Users");

app.MapPut("/api/users/{id}/role", async (Guid id, UpdateRoleRequest request, IValidator<UpdateRoleRequest> validator, IMediator _mediator) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors
            .Select(e => new { e.PropertyName, e.ErrorMessage }));
    }

    var response = await _mediator.Send(request);

    if (response is null || !response.Success)
    {
        return Results.BadRequest(new { Message = response?.Message ?? "Ocorreu um erro ao processar a solicitação." });
    }

    return Results.Ok(response);
}).RequireAuthorization("AdminPolicy").WithTags("Users");

app.MapGet("/api/users", async (IUserQuery _userQuery) =>
{
    var users = await _userQuery.GetAllUsersAsync();

    return users is not null ? Results.Ok(users) : Results.NotFound();
}).RequireAuthorization("AdminPolicy").WithTags("Users");

app.MapDelete("/api/users/{id}", async (Guid id, IUserRepository _userRepository) =>
{
    var deleted = await _userRepository.DeleteUserAsync(id);
    return deleted
        ? Results.NoContent()
        : Results.NotFound();
}).RequireAuthorization("AdminPolicy").WithTags("Users");
#endregion

#region Auth Endpoints
app.MapPost("/api/login", async (LoginRequest request, IValidator<LoginRequest> validator, IMediator mediator) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors
            .Select(e => new { e.PropertyName, e.ErrorMessage }));
    }

    var response = await mediator.Send(request);

    if (!response.Success)
        return Results.Json(new { response.Message }, statusCode: StatusCodes.Status401Unauthorized);

    return Results.Ok(new
    {
        response.Token,
        response.UserId,
        response.Email,
        response.Message
    });
}).AllowAnonymous().WithTags("Auth");
#endregion

#region Genre Endpoints
app.MapGet("/api/genres", () =>
{
    var genres = Enum.GetValues(typeof(GenreEnum))
                     .Cast<GenreEnum>()
                     .Select(g => new
                     {
                         Id = (int)g,
                         Name = g.ToString()
                     });

    return Results.Ok(genres);
}).RequireAuthorization().WithTags("Genres");
#endregion

#region Game Endpoints
app.MapGet("/api/games/{id}", async (Guid id, IGameQuery _gameQuery) =>
{
    var game = await _gameQuery.GetByIdGameAsync(id);

    return game is not null ? Results.Ok(game) : Results.NotFound();
}).RequireAuthorization().WithTags("Games");

app.MapGet("/api/games", async (IGameQuery _gameQuery) =>
{
    var games = await _gameQuery.GetAllGamesAsync();

    return games is not null ? Results.Ok(games) : Results.NotFound();
}).RequireAuthorization().WithTags("Games");

app.MapPost("/api/games", async (CreateGameRequest request, IValidator<CreateGameRequest> validator, IMediator _mediator) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors
            .Select(e => new { e.PropertyName, e.ErrorMessage }));
    }

    var response = await _mediator.Send(request);

    if (response is null || !response.Success)
    {
        return Results.BadRequest(new { Message = response?.Message ?? "Ocorreu um erro ao processar a solicitação." });
    }

    return Results.Created($"/api/games/{response.GameId}", response);
}).RequireAuthorization("AdminPolicy").WithTags("Games");

app.MapPost("/api/games/buy", async (BuyGameRequest request, IValidator<BuyGameRequest> validator, IMediator _mediator) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors
            .Select(e => new { e.PropertyName, e.ErrorMessage }));
    }

    var response = await _mediator.Send(request);

    if (response is null || !response.Success)
    {
        return Results.BadRequest(new { Message = response?.Message ?? "Ocorreu um erro ao processar a solicitação." });
    }

    return Results.Created($"/api/users/{response.UserId}/games", response);
}).RequireAuthorization().WithTags("Games");

app.MapDelete("/api/games/{id}", async (Guid id, IGameRepository _gameRepository) =>
{
    var deleted = await _gameRepository.DeleteGameAsync(id);

    return deleted
        ? Results.NoContent()
        : Results.NotFound();
}).RequireAuthorization("AdminPolicy").WithTags("Games");
#endregion

#region Promotion Endpoints
app.MapGet("/api/promotions/{id}", async (Guid id, IPromotionQuery _promotionQuery) =>
{
    var promotion = await _promotionQuery.GetByIdPromotionAsync(id);

    return promotion is not null ? Results.Ok(promotion) : Results.NotFound();
}).RequireAuthorization().WithTags("Promotions");

app.MapGet("/api/promotions", async (IPromotionQuery _promotionQuery) =>
{
    var games = await _promotionQuery.GetAllPromotionsAsync();

    return games is not null ? Results.Ok(games) : Results.NotFound();
}).RequireAuthorization("AdminPolicy").WithTags("Promotions");

app.MapPost("/api/promotions", async (CreatePromotionRequest request, IValidator<CreatePromotionRequest> validator, IMediator _mediator) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors
            .Select(e => new { e.PropertyName, e.ErrorMessage }));
    }

    var response = await _mediator.Send(request);

    if (response is null || !response.Success)
    {
        return Results.BadRequest(new { Message = response?.Message ?? "Ocorreu um erro ao processar a solicitação." });
    }

    return Results.Created($"/api/promotions/{response.PromotionId}", response);
}).RequireAuthorization("AdminPolicy").WithTags("Promotions");

app.MapDelete("/api/promotions/{id}", async (Guid id, IPromotionRepository _promotionRepository) =>
{
    var deleted = await _promotionRepository.DeletePromotionAsync(id);

    return deleted
        ? Results.NoContent()
        : Results.NotFound();
}).RequireAuthorization("AdminPolicy").WithTags("Promotions");
#endregion

#region Middleware Pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseLogMiddleware();
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpMetrics();
app.MapMetrics();

app.MapControllers();


#endregion

app.Run();
#endregion
