using Bencodex.Types;
using Libplanet.Action;

namespace Telescope
{
    /// <summary>
    /// A mock <see cref="IAction"/>.
    /// </summary>
    public class MockAction : IAction
    {
        /// <inheritdoc/>
        public IValue PlainValue { get; private set; }

        public MockAction()
        {
            PlainValue = Bencodex.Types.Null.Value;
        }

        /// <inheritdoc/>
        public void LoadPlainValue(IValue plainValue)
        {
            PlainValue = plainValue;
        }

        /// <inheritdoc/>
        public IAccountStateDelta Execute(IActionContext context)
        {
            throw new InvalidOperationException("Execution is not allowed.");
        }
    }
}
