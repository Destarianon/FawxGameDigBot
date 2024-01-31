using FawxGameDigBot;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddSingleton<FawxGameDigBot.GameDig>();
builder.Services.AddSingleton<FawxGameDigBot.Discord>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();