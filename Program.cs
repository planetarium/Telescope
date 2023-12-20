using Cocona;
using Libplanet.Action;
using Libplanet.Action.Loader;
using Libplanet.Action.State;
using Libplanet.Blockchain;
using Libplanet.RocksDBStore;
using Libplanet.Store;
using Libplanet.Types.Blocks;
using Telescope.Gui;
using Terminal.Gui;

namespace Telescope
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CoconaLiteApp.Run<Program>(args);
        }

        public void Run(
            [Option('p', Description = "Path to chain storage with its scheme specified.")]
            string path,
            [Option('i', Description = "Index of the block to show.")]
            long? blockIndex = null)
        {
            Uri uri = new Uri($"{path}");

            (IStore store, IStateStore stateStore) = LoadStores(uri);
            WrappedBlockChain blockChain = LoadBlockChain(store, stateStore);

            Application.Init();

            Views views = new Views(blockChain);
            Menus menus = new Menus(views);
            views.TopView.Add(menus.MenuBar);
            if (blockIndex is { } bi)
            {
                Dialogs.IndexSearchAction(views, bi.ToString());
            }

            Application.Run(views.TopView);
            Application.Shutdown();
            store.Dispose();
            stateStore.Dispose();
        }

        private (IStore, IStateStore) LoadStores(Uri uri)
        {
#pragma warning disable CS0168 // The variable '_' is declared but never used
            // FIXME: This is used to forcefully load the RocksDBStore assembly
            // so that StoreLoaderAttribute can find the appropriate loader.
            RocksDBStore _;
#pragma warning restore CS0168

            var loaded = StoreLoaderAttribute.LoadStore(uri);

            if (loaded is { } l)
            {
                return (l.Store, l.StateStore);
            }
            else
            {
                throw new NullReferenceException("Failed to load store");
            }
        }

        private WrappedBlockChain LoadBlockChain(IStore store, IStateStore stateStore)
        {
            Guid canon = store.GetCanonicalChainId()
                ?? throw new NullReferenceException(
                    $"Failed to load canonical chain from {nameof(store)}");

            BlockHash genesisHash = store.IndexBlockHash(canon, 0)
                ?? throw new NullReferenceException(
                    $"Failed to load genesis block from {nameof(store)}");
            Block genesis = store.GetBlock(genesisHash);
            IBlockChainStates blockChainStates = new BlockChainStates(store, stateStore);
            IActionLoader actionLoader = new SingleActionLoader(typeof(MockAction));
            IActionEvaluator actionEvaluator = new ActionEvaluator(
                policyBlockActionGetter: _ => null,
                stateStore: stateStore,
                actionTypeLoader: actionLoader);

            var blockChain = new BlockChain(
                policy: new Libplanet.Blockchain.Policies.BlockPolicy(),
                stagePolicy: new Libplanet.Blockchain.Policies.VolatileStagePolicy(),
                store: store,
                stateStore: stateStore,
                genesisBlock: genesis,
                blockChainStates: blockChainStates,
                actionEvaluator: actionEvaluator);
            return new WrappedBlockChain(blockChain);
        }
    }
}
