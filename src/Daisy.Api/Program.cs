// Copyright (c) 2025 Rui Filipe Rodrigues Vaz
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Text;
using Daisy.Api.Models;
using Daisy.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "DaisyApi",
        ValidAudience = jwtSettings["Audience"] ?? "DaisyApiClients",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

// Add OData services
builder.Services.AddControllers()
    .AddOData(options => options
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .Count()
        .SetMaxTop(100)
        .AddRouteComponents("odata", GetEdmModel()));

// Register application services
builder.Services.AddSingleton<IImpulseService, ImpulseService>();
builder.Services.AddSingleton<IDaisyComponentService, DaisyComponentService>();

// Add API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Daisy MCP API",
        Version = "v1",
        Description = "Model Context Protocol compliant API for Daisy workflow orchestration engine with OAuth 2.0 and OData support",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Daisy Project",
            Url = new Uri("https://github.com/ruifrvaz/Daisy-m4")
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "Apache 2.0",
            Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0")
        }
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Daisy MCP API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static IEdmModel GetEdmModel()
{
    var modelBuilder = new ODataConventionModelBuilder();
    
    modelBuilder.EntitySet<ImpulseDto>("Impulses").EntityType.HasKey(i => i.Id);
    modelBuilder.EntitySet<ReceiverDto>("Receivers").EntityType.HasKey(r => r.Name);
    modelBuilder.EntitySet<AbilityDto>("Abilities").EntityType.HasKey(a => a.Name);
    modelBuilder.EntitySet<TransmitterDto>("Transmitters").EntityType.HasKey(t => t.Name);
    modelBuilder.EntitySet<WorkflowDto>("Workflows").EntityType.HasKey(w => w.Name);
    
    return modelBuilder.GetEdmModel();
}

// Make the Program class accessible to tests
public partial class Program { }
