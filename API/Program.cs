using API.Exstenssions;
using API.Seed;
using Core.ServicesInterfaces;
using Met.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using Infrastructure.Services;
using Met.Services;
using API.Filters;
using Microsoft.AspNetCore.Mvc;
using API.Authorizing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Infrastructure.Repository;
using Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


TokenValidationParameters? tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding
                            .ASCII
                            .GetBytes(builder.Configuration["JWT:secret"])),
    ValidIssuer = builder.Configuration["JWT:Issuer"],
    ValidateIssuer = true,
    ValidAudience = builder.Configuration["JWT:Audience"],

    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero,


};


builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddDbContext<AppIdentityDbContext>(
    x => x.UseSqlite(builder.Configuration.GetConnectionString("IdentityConnection")));


builder.Services.AddMet(builder.Configuration);


builder.Services.AddScoped<LoginValidationAttribute>();
builder.Services.Configure<MvcOptions>(opts => opts.Filters.Add<HttpsOnlyAttribute>());


builder.Services
.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{

    options.SaveToken = true;
    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters =
       tokenValidationParameters;
});




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerDocumentation();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
    });
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(x =>
{
    x.TokenLifespan = TimeSpan.FromDays(1);
});

//builder.Services.AddAuthorization(options=>{
//    options.AddPolicy("Admin", policy=>policy.RequireClaim("admin","true") );
//});

//builder.Services.AddAuthorization(options=>{
//    options.AddPolicy("EditAdmin", policy=>policy.
//    AddRequirements(new ManageAdminRolesAndClaimsRequirement()) );
//});


//builder.Services.AddAuthorization(options=>{
//    options.AddPolicy("Sample", policy=>policy.
//    AddRequirements(new ManageAdminRolesAndClaimsRequirement()) );

//    options.FallbackPolicy = new AuthorizationPolicy(
//        new IAuthorizationRequirement[]{
//            new  NameAuthorizationRequirement("bob"),
//        },Enumerable.Empty<String>()
//    );

//});


builder.Services.AddSingleton<IAuthorizationHandler,
canEditOnlyOtherAdminRolesAndClaimsHandler>();


builder.Services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();

// builder.Services.AddAuthorization(options=>{
//    options.AddPolicy("SuperAssertion", policy=>policy.
//    RequireAssertion(x=>x.User.IsInRole("")  && x.User.HasClaim(c=>c.Type=="" && c.Value=="")
//    || x.User.IsInRole("Super")) );

//    options.InvokeHandlersAfterFailure= false;


//});




builder.Services.ConfigureApplicationCookie(x =>
x.AccessDeniedPath = new PathString("/api/AccountController/AccessDenied"));
builder.Services.AddScoped<IFileStorageRepository, AppStorageRepository>();

var app = builder.Build();




//  ...................................................
// using var scope = app.Services.CreateScope();

// var services = scope.ServiceProvider;


// try{

//     var context = services.GetRequiredService<AppIdentityDbContext>();
//      context.Database.Migrate();
// }
// catch(Exception ex){

//     var logger = services.GetRequiredService<ILogger<Program>>();
//     logger.LogError(ex,"Error in Migration");
// }

// ........................................................


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();

    app.UseSwaggerDocumentation();
    app.UseHttpsRedirection();

}


app.UseStaticFiles();

app.UseCors(x => x.AllowAnyHeader()
    .AllowAnyMethod().AllowAnyOrigin()
    );


app.UseAuthentication();


app.UseAuthorization();

app.MapControllers();


app.SeedRolesToDb().Wait();
app.Run();
