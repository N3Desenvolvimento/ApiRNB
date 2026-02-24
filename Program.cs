using API_RNB.Conexao;
using API_RNB.Repository;
using API_RNB.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<VendasHookRepository>();
builder.Services.AddScoped<FirebirdDatabase>();
builder.Services.AddScoped<WebhookService>();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<VendasHookWorker>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy =>
        {
            policy
                .AllowAnyOrigin() // Permitir qualquer origem
                .AllowAnyMethod() // Permitir qualquer m�todo HTTP (GET, POST, etc.)
                .AllowAnyHeader(); // Permitir qualquer cabe�alho
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
/*
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
*/
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
