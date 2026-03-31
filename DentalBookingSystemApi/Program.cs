var builder = WebApplication.CreateBuilder(args);

// Database 
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName);
});

builder.Services.AddSignalR();
builder.Services.AddScoped<EmailService>();
builder.Services.AddHostedService<ReminderBackgroundService>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

EndpointRegister.MapAllEndpoints(app);

app.MapHub<BookingHub>("/hubs/booking");

app.Run();