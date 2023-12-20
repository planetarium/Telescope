using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;

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

        /// <inheritdoc cref="IAction.LoadPlainValue"/>
        public void LoadPlainValue(IValue plainValue)
        {
            PlainValue = plainValue;
        }

        /// <inheritdoc cref="IAction.Execute/>
        public IAccount Execute(IActionContext context)
        {
            throw new InvalidOperationException("Execution is not allowed.");
        }
    }
}
