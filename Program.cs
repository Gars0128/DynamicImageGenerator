using ImageGeneratorAPI;


var builder = WebApplication.CreateBuilder(args);


// add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<ImageGeneratorAPI.Middleware.ErrorHandlingMiddleware>();


app.Run();
