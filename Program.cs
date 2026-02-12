using API_RNB.Conexao;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<FirebirdDatabase>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // Permitir qualquer origem
              .AllowAnyMethod()   // Permitir qualquer método HTTP (GET, POST, etc.)
              .AllowAnyHeader();  // Permitir qualquer cabeçalho
    });
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
