using BuildingBlocks.Application.InMemoryBus;
using BuildingBlocks.Auth;
using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.Enums;
using BuildingBlocks.Shared.InfrastructureInterfaces.Persistence.EFCore;
using Ecommerce.Services.Orders.Application.Features.Orders.Dtos;
using Ecommerce.Services.Orders.Application.Services;
using Ecommerce.Services.Orders.Domain;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services.Orders.Application.Features.Commands.CreateOrder;
public class CreateOrderCommandHandler(
    ICartService cartService,
    IEfUnitOfWork unitOfWork,
    ILogger<CreateOrderCommandHandler> logger, IMapper mapper)
    : CommandHandler<CreateOrderCommand, CustomerOrderResponse>
{
    protected override async Task<Result<CustomerOrderResponse>> HandleCommandAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var customerId = command.CustomerId;
        try
        {
            logger.LogInformation("Bắt đầu tạo đơn hàng (checkout) cho khách hàng: {CustomerId}", customerId);

            var cartResult = await cartService.GetCartByCustomerId(customerId);

            if (!cartResult.IsSuccess)
            {
                return  Result<CustomerOrderResponse>.Failure(cartResult.Errors);
            }
            
            var cartResponse = cartResult.Value;

            if (cartResponse == null || cartResponse.Items.Count == 0)
            {
                return Result<CustomerOrderResponse>.Failure("Giỏ hàng trống, không thể thanh toán", EErrorCode.ValidationErrors);
            }

            var errors = new List<string>();
            foreach (var cartItem in cartResponse.Items)
            {
                if (cartItem.AvailableStocks < cartItem.Quantity)
                {
                    errors.Add($"Sản phẩm {cartItem.ProductName} - {cartItem.VariantName} chỉ còn {cartItem.AvailableStocks} sản phẩm trong kho, không đủ để đặt hàng");
                }
            }

            if (errors.Any())
            {
                return Result<CustomerOrderResponse>.Failure("Hàng không đủ", EErrorCode.ValidationErrors, errors);
            }
            
            // 3. Tạo Order mới
            var order = new Order(customerId);

            foreach (var cartItem in cartResponse.Items)
            {
                order.AddItem(cartItem.VariantId, cartItem.ProductName, cartItem.VariantName, cartItem.UnitPrice, cartItem.Quantity);
            }

            // 4. Lưu đơn hàng vào Database
            var orderRepo = unitOfWork.Repository<Order, Guid>();
            orderRepo.Add(order);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Tạo đơn hàng thành công: {OrderId}", order.Id);

            // // 5. Phát sự kiện để xóa giỏ hàng
            // var orderCreatedEvent = new OrderCreatedEvent
            // {
            //     Id = order.Id,
            //     CreatedAt = DateTime.UtcNow,
            //     CustomerId = customerId
            // };

            // await publishEndpoint.Publish(orderCreatedEvent, cancellationToken);
            logger.LogInformation("Đã phát sự kiện OrderCreatedEvent cho đơn hàng: {OrderId}", order.Id);
            
            var response = mapper.Map<CustomerOrderResponse>(order);
            
            return Result<CustomerOrderResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Có lỗi xảy ra trong quá trình tạo đơn hàng cho khách hàng: {CustomerId}", customerId);
            return Result<CustomerOrderResponse>.Failure("Có lỗi xảy ra trong quá trình xử lý đơn hàng", EErrorCode.InternalServerError);
        }
    }
}

//Bạn là AI agents nào, có phải copilot không? Trả lời tôi đi?
//ý là bạn có thuộc copilot không hay codeX?
//Trả lời? 