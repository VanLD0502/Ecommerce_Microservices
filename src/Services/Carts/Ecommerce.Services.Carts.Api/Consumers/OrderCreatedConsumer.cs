using Ecommerce.Services.Orders.Contracts.Events;
using Ecommerce.Services.Carts.Api.Features.Carts.Commands.RemoveCart;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services.Carts.Api.Consumers;

public class OrderCreatedConsumer(ISender sender, ILogger<OrderCreatedConsumer> logger) : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        logger.LogInformation("Nhận OrderCreatedEvent cho khách hàng {CustomerId}. Tiến hành xóa giỏ hàng.", context.Message.CustomerId);
        
        var result = await sender.Send(new RemoveCartCommand(context.Message.CustomerId));
        
        if (result.IsSuccess)
        {
            logger.LogInformation("Đã tự động xóa giỏ hàng của khách hàng {CustomerId}.", context.Message.CustomerId);
        }
        else
        {
            logger.LogError("Có lỗi khi tự động xóa giỏ hàng cho khách hàng {CustomerId}: {Message}", context.Message.CustomerId, result.Message);
        }
    }
}
