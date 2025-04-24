using Microsoft.Extensions.Options;
using Stripe;
using TestApplication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

// Register StripeClient as a singleton
builder.Services.AddSingleton<IStripeClient>(sp =>
{
    var stripeSettings = sp.GetRequiredService<IOptions<StripeSettings>>().Value;
    return new StripeClient(stripeSettings.SecretKey);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<CustomerRepo>();
builder.Services.AddSingleton<DeliveryDateRepo>();
builder.Services.AddSingleton<ProductRepo>();
builder.Services.AddSingleton<OrderRepo>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
