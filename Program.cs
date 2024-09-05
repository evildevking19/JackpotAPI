using Npgsql;
using ApiServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Convert all URLs to lowercase
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var connectionString = builder.Configuration.GetConnectionString("PostgreDB");
builder.Services.AddScoped((provider) => new NpgsqlConnection(connectionString));
builder.Services.AddScoped<IChampionService, ChampionService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IPotService, PotService>();
builder.Services.AddScoped<IAwardService, AwardService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();
