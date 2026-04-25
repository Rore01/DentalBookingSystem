var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services
    .AddFluentEmail(builder.Configuration["Email:From"])
    .AddLiquidRenderer()
    .AddSmtpSender(new System.Net.Mail.SmtpClient
    {
        Host = builder.Configuration["Email:SmtpHost"]!,
        Port = int.Parse(builder.Configuration["Email:SmtpPort"]!),
        EnableSsl = true,
        Credentials = new System.Net.NetworkCredential(
            builder.Configuration["Email:Username"],
            builder.Configuration["Email:Password"]
        )
    });

builder.Services.AddScoped<EmailService>();

builder.Services.AddSignalR();

builder.Services.AddHostedService<ReminderBackgroundService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5107", "https://localhost:7091")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors();

app.MapHub<BookingHub>("/hubs/booking");

EndpointRegister.MapAllEndpoints(app);

app.Run();