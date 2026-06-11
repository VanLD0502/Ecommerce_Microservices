// using MassTransit;
// using MassTransit.Api;
// using MassTransit.Api.Settings;
// using Microsoft.AspNetCore.Identity.UI.Services;
// using Microsoft.Extensions.Options;
// using Microsoft.VisualBasic;
//
// var builder = WebApplication.CreateBuilder(args);
//
// // Add services to the container.
// // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
//
//
//
// var app = builder.Build();
//
// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }
//
// app.UseHttpsRedirection();
//
// // app.MapGet("/publish", async (IPublishEndpoint publishEndpoint) =>
// // {
// //     // await publishEndpoint.Publish(new HelloMessage() { Message = "Chịch em đi"});
// //     
// //     return Results.Ok("Hello World!");
// // });
// //
// // app.MapGet("/pl", async (string msg, ISendEndpointProvider sender) =>
// // {
// //     var endpoint = await sender.GetSendEndpoint(
// //         new Uri("queue:uuu"));
// //
// //     await endpoint.Send(new HelloMessage() { Message = msg });
// //
// //     return Results.Ok("Sent");
// // });
//
// app.Run();