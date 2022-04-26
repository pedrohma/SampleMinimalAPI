using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using SampleMinimal.API.MapProfiles;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
Log.Information("Starting up...");

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add database connection
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sample API", Version = "v1" }); });

// Add AutoMapper
var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new TodoProfile());
});

IMapper autoMapper = mappingConfig.CreateMapper();

// Add Services
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton(autoMapper);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample API V1"));
}

app.UseHttpsRedirection();

app.MapGet("/todo", async (IRepository<Todo> _todoRepository, ILoggerFactory _loggerFactory, IMapper _mapper) =>
{
    var logger = _loggerFactory.CreateLogger<Todo>();
    logger.LogInformation("Getting all Todo items");
    return Results.Ok(_mapper.Map<TodoDTO>(await _todoRepository.GetAllAsync()));
}).WithName("GetAll");

app.MapGet("/todo/{id}", async (int id, IRepository<Todo> _todoRepository, ILoggerFactory _loggerFactory, IMapper _mapper) =>
{
    var logger = _loggerFactory.CreateLogger<Todo>();
    logger.LogInformation("Getting single Todo item");
    return Results.Ok(_mapper.Map<TodoDTO>(await _todoRepository.GetByIdAsync(id)));
}).WithName("Get");

app.MapPost("/todo", async ([FromBody] TodoDTO todo, IRepository<Todo> _todoRepository, ILoggerFactory _loggerFactory, IMapper _mapper) =>
{
    if (todo == null) return Results.BadRequest("No ToDo item.");

    var logger = _loggerFactory.CreateLogger<Todo>();
    logger.LogInformation("Adding Todo item");
    bool result = await _todoRepository.AddAsync(_mapper.Map<Todo>(todo));
    if (result) return Results.Ok("Add successful");
    return Results.StatusCode(StatusCodes.Status500InternalServerError);
}).WithName("Add");

app.MapPut("/todo/{id}", async (int id, [FromBody] TodoDTO todo, IRepository<Todo> _todoRepository, ILoggerFactory _loggerFactory, IMapper _mapper) =>
{
    if (id < 1 || todo == null) return Results.BadRequest("Unknown item, or update incorrect");

    var logger = _loggerFactory.CreateLogger<Todo>();
    logger.LogInformation("Updating Todo item");
    Todo existingTodo = await _todoRepository.GetByIdAsync(id);
    _mapper.Map(todo, existingTodo);
    bool result = await _todoRepository.UpdateAsync(existingTodo);
    if (result) return Results.Ok("Update successful");
    return Results.StatusCode(StatusCodes.Status500InternalServerError);
}).WithName("Update");

app.MapDelete("/todo/{id}", async (int id, IRepository<Todo> _todoRepository, ILoggerFactory _loggerFactory) =>
{
    if (id < 1) return Results.BadRequest("Unknown item");
    Todo todo = await _todoRepository.GetByIdAsync(id);
    if (todo == null) return Results.BadRequest("Item not found");
    bool result = await _todoRepository.DeleteAsync(todo);
    if (result) return Results.Ok("Delete successful");
    return Results.StatusCode(StatusCodes.Status500InternalServerError);
}).WithName("Delete");

app.Run();