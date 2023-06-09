using ElectricityDataAggregator.Common.ApiErrorHandling.Models;

namespace ElectricityDataAggregator.Common.ApiErrorHandling.Contracts
{
    public interface IExceptionHandlerBase
    {
    }

    public interface IExceptionHandler<TException> : IExceptionHandlerBase
        where TException : Exception
    {
        void Handle(ApiProblemContext<TException> ctx);
    }
}
