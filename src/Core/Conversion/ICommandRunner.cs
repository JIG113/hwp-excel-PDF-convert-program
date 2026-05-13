using System.Threading;
using System.Threading.Tasks;

namespace Converter.Core.Conversion;

public interface ICommandRunner
{
    Task<CommandResult> RunAsync(string fileName, string arguments, int timeoutSeconds, CancellationToken cancellationToken);
}
