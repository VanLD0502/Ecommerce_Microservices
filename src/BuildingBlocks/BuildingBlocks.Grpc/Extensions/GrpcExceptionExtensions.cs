using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.Enums;
using Grpc.Core;

namespace BuildingBlocks.Grpc.Extensions;

public static class GrpcExceptionExtensions
{
    // 1. Hàm ánh xạ mã lỗi EErrorCode sang gRPC StatusCode
    private static StatusCode MapToGrpcStatusCode(this EErrorCode errorCode)
    {
        return errorCode switch
        {
            EErrorCode.Success => StatusCode.OK,
            EErrorCode.InvalidArgument => StatusCode.InvalidArgument,
            EErrorCode.Unauthorized => StatusCode.Unauthenticated,
            EErrorCode.Forbidden => StatusCode.PermissionDenied,
            EErrorCode.NotFound => StatusCode.NotFound,
            EErrorCode.ValidationErrors => StatusCode.FailedPrecondition, // Hoặc InvalidArgument tùy thiết kế
            _ => StatusCode.Internal
        };
    }
    
    private static EErrorCode ToEErrorCode(this StatusCode statusCode)
    {
        return statusCode switch
        {
            StatusCode.OK => EErrorCode.Success,
            StatusCode.InvalidArgument => EErrorCode.InvalidArgument,
            StatusCode.Unauthenticated => EErrorCode.Unauthorized,
            StatusCode.PermissionDenied => EErrorCode.Forbidden,
            StatusCode.NotFound => EErrorCode.NotFound,
            StatusCode.FailedPrecondition => EErrorCode.ValidationErrors,
            _ => EErrorCode.InternalServerError
        };
    }

    public static RpcException ToRpcException<T>(this Result<T> result)
    {
        var grpcStatusCode = result.ErrorCode.MapToGrpcStatusCode();
        return new RpcException(new Status(grpcStatusCode, result.Message));
    }

    public static RpcException ToRpcException(this Result result)
    {
        var grpcStatusCode = result.ErrorCode.MapToGrpcStatusCode();
        return new RpcException(new Status(grpcStatusCode, result.Message));
    }
    
    
    public static Result<T> ToResultFailure<T>(this RpcException ex)
    {
        var errorCode = ex.StatusCode.ToEErrorCode();
        // ex.Status.Detail chính là cái Message mà Server ném ra trong RpcException
        return Result<T>.Failure(errorCode, ex.Status.Detail!);
    }
    
    // 3. Tự động chuyển RpcException thành Result dạng Failure
    public static Result ToResultFailure(this RpcException ex)
    {
        var errorCode = ex.StatusCode.ToEErrorCode();
        return Result.Failure(ex.Status.Detail!,  errorCode);
    }
}