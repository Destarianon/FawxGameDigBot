using FawxGameDigBot;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddSingleton<GameDig>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();