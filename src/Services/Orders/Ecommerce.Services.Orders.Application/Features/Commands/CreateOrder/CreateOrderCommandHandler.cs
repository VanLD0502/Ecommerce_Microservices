using BuildingBlocks.Application.InMemoryBus;
using BuildingBlocks.Grpc.Services;
using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.Enums;
using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using BuildingBlocks.Shared.InfrastructureInterfaces.Persistence.EFCore;
using Ecommerce.Services.Orders.Application.Commons.Dtos.Orders;
using Ecommerce.Services.Orders.Contracts.Events;
using Ecommerce.Services.Orders.Domain;
using MapsterMapper;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services.Orders.Application.Features.Commands.CreateOrder;

public record CreateOrderCommand(int CustomerId) : ICommand<CustomerOrderResponse>;

public class CreateOrderCommandHandler(
    CartGrpc.CartGrpcClient cartClient,
    ProductGrpc.ProductGrpcClient productClient,
    IEfUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint,
    ILogger<CreateOrderCommandHandler> logger, IMapper mapper)
    : CommandHandler<CreateOrderCommand, CustomerOrderResponse>
{
    protected override async Task<Result<CustomerOrderResponse>> HandleCommandAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Bắt đầu tạo đơn hàng (checkout) cho khách hàng: {CustomerId}", command.CustomerId);
            
            var cartResponse = await cartClient.GetCartByCustomerIdAsync(
                new GetCartByIdRequest() { CustomerId = command.CustomerId },
                cancellationToken: cancellationToken);

            if (cartResponse == null || cartResponse.Items.Count == 0)
            {
                return Result<CustomerOrderResponse>.Failure("Giỏ hàng trống, không thể thanh toán", EErrorCode.ValidationErrors);
            }
            
            var validateRequest = new ValidateProductsRequest();
            foreach (var item in cartResponse.Items)
            {
                validateRequest.Items.Add(new ValidateProductItemDto
                {
                    Id = item.ProductId,
                    Price = item.UnitPrice,
                    Quantity = item.Quantity
                });
            }

            var validationResult = await productClient.ValidateProductsAsync(validateRequest, cancellationToken: cancellationToken);

            var errors = new List<string>();
            for (int i = 0; i < cartResponse.Items.Count; i++)
            {
                var cartItem = cartResponse.Items[i];
                var validatedItem = validationResult.Items[i];

                if (validatedItem.ErrorStatus == ProductValidatedErrorStatus.None)
                {
                    continue;
                }

                string errorMsg = validatedItem.ErrorStatus switch
                {
                    ProductValidatedErrorStatus.PriceChanged =>
                        $"Sản phẩm {validatedItem.Name} có giá thay đổi từ {cartItem.UnitPrice} sang {validatedItem.Price}",
                    ProductValidatedErrorStatus.OutOfStock =>
                        $"Số lượng mua sản phẩm {validatedItem.Name} lớn hơn lượng tồn kho",
                    ProductValidatedErrorStatus.NotFound =>
                        $"Sản phẩm {cartItem.ProductName} không tìm thấy hoặc đã bị xóa",
                    _ => "Lỗi không xác định"
                };

                errors.Add(errorMsg);
            }

            if (errors.Count > 0)
            {
                var combinedError = string.Join("; ", errors);
                logger.LogWarning("Tạo đơn hàng thất bại do lỗi validation: {Errors}", combinedError);
                return Result<CustomerOrderResponse>.Failure(combinedError, EErrorCode.ValidationErrors);
            }

            // 3. Tạo Order mới
            var order = new Order(command.CustomerId)
            {
                Id = Guid.NewGuid()
            };

            foreach (var item in cartResponse.Items)
            {
                var unitPrice = decimal.Parse(item.UnitPrice, System.Globalization.CultureInfo.InvariantCulture);
                order.AddItem(
                    Guid.Parse(item.ProductId),
                    item.ProductName,
                    unitPrice,
                    item.Quantity
                );
            }

            // 4. Lưu đơn hàng vào Database
            var orderRepo = unitOfWork.Repository<Order, Guid>();
            orderRepo.Add(order);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Tạo đơn hàng thành công: {OrderId}", order.Id);

            // 5. Phát sự kiện để xóa giỏ hàng
            var orderCreatedEvent = new OrderCreatedEvent
            {
                Id = order.Id,
                CreatedAt = DateTime.UtcNow,
                CustomerId = command.CustomerId
            };

            await publishEndpoint.Publish(orderCreatedEvent, cancellationToken);
            logger.LogInformation("Đã phát sự kiện OrderCreatedEvent cho đơn hàng: {OrderId}", order.Id);
            
            var response = mapper.Map<CustomerOrderResponse>(order);
            
            return Result<CustomerOrderResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Có lỗi xảy ra trong quá trình tạo đơn hàng cho khách hàng: {CustomerId}", command.CustomerId);
            return Result<CustomerOrderResponse>.Failure("Có lỗi xảy ra trong quá trình xử lý đơn hàng", EErrorCode.InternalServerError);
        }
    }
}

//Bạn là AI agents nào, có phải copilot không? Trả lời tôi đi?
//ý là bạn có thuộc copilot không hay codeX?
//Trả lời? 