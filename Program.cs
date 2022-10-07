using Cocona;
using Libplanet.Blockchain;
using Libplanet.Blocks;
using Libplanet.RocksDBStore;
using Libplanet.Store;
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
            string path)
        {
            Uri uri = new Uri($"{path}");

            (IStore store, IStateStore stateStore) = LoadStores(uri);
            WrappedBlockChain blockChain = LoadBlockChain(store, stateStore);

            Application.Init();

            Views views = new Views(blockChain);
            Menus menus = new Menus(views);
            views.TopView.Add(menus.MenuBar);

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
            Block<MockAction> genesis = store.GetCanonicalGenesisBlock<MockAction>();

            var blockChain = new BlockChain<MockAction>(
                policy: new Libplanet.Blockchain.Policies.BlockPolicy<MockAction>(),
                stagePolicy: new Libplanet.Blockchain.Policies.VolatileStagePolicy<MockAction>(),
                store: store,
                stateStore: stateStore,
                genesisBlock: genesis);
            return new WrappedBlockChain(blockChain);
        }
    }
}
