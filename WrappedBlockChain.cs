using System.Collections;
using Libplanet.Blockchain;
using Libplanet.Common;
using Libplanet.Crypto;
using Libplanet.Types.Blocks;

namespace Telescope
{
    public class WrappedBlockChain : IList
    {
        private BlockChain _blockChain;
        private object _syncRoot;

        public WrappedBlockChain(BlockChain blockChain)
        {
            _blockChain = blockChain;
            _syncRoot = new object();
        }

        public int IndexOf(object? obj) => throw new NotImplementedException("Indexing is not allowed.");

        public void Insert(int index, object? obj) => throw new NotImplementedException("Inserting is not allowed.");

        public void RemoveAt(int index) => throw new NotImplementedException("Removing is not allowed.");

        // FIXME: Index retrieval should use long.
        public object? this[int index]
        {
            get
            {
                return new WrappedBlock(_blockChain[index]);
            }
            set
            {
                throw new NotImplementedException("Setting is not allowed.");
            }
        }

        public WrappedBlock this[byte[] blockHash]
        {
            get
            {
                return new WrappedBlock(_blockChain[new BlockHash(blockHash)]);
            }
            set
            {
                throw new NotImplementedException("Setting is not allowed.");
            }
        }

        public int Add(object? obj) => throw new NotImplementedException("Adding is not allowed.");

        public void Clear() => throw new NotImplementedException("Clearing is not allowed.");

        public bool Contains(object? obj) => throw new NotImplementedException("Checking containment is not allowed.");

        // FIXME: Forced casting due to IList.
        public int Count => (int)_blockChain.Count;

        public void CopyTo(Array objs, int count) => throw new NotImplementedException("Copying is not allowed.");

        public void Remove(object? obj) => throw new NotImplementedException("Removing is not allowed.");

        public bool IsReadOnly => true;

        public bool IsFixedSize => true;

        public bool IsSynchronized => true;

        public object SyncRoot => _syncRoot;

        public IEnumerator<WrappedBlock> GetEnumerator() => throw new NotImplementedException("Enumerating is not allowed.");

        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException("Enumerating is not allowed.");

        public WrappedState GetState(string hash, string address)
        {
            var state = _blockChain.GetState(new Address(address), new BlockHash(ByteUtil.ParseHex(hash)));
            return state is { } s
                ? new WrappedState(s)
                : throw new NullReferenceException("Failed to fetch state.");
        }

        public bool HasState(long index)
        {
            // NOTE: Better way would be to check ITrie.Recroded, but
            // BlockChain does not allow direct access to IStateStore.
            try
            {
                _ = _blockChain.GetAccountState(_blockChain[index].StateRootHash);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
