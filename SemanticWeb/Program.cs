using SemanticWeb.DB;
using Microsoft.EntityFrameworkCore;
using SemanticWeb.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy( builder =>
                      {
                          builder.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});
// Add services to the container.

builder.Services.AddDbContext<UserContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient<IDBPediaHelper, DBPediaHelper>();
builder.Services.AddTransient<ITokenHelper, TokenHelper>();
 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}  

app.UseHttpsRedirection(); 
app.UseAuthorization();
app.MapControllers();
app.UseCors();
app.UseStaticFiles();
app.UseRouting();
 

app.Run();
