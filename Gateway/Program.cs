var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

//IConfiguration configuration = new ConfigurationBuilder()
//    .AddJsonFile("ocelot.json")
//    .Build();
//builder.Services.AddOcelot(configuration);

var app = builder.Build();

//app.UseHttpsRedirection();

//app.UseOcelot().Wait();
app.MapReverseProxy();

app.Run();
