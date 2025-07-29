using MediatR;

namespace WordStreamAnalyser.Application;

public record ProcessStreamCommand : IRequest<Unit>;