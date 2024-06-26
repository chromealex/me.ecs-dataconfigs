#if !ENTITIES_GROUP_DISABLED
namespace ME.ECS.Tests {

    public class Tests_Entities_EntitiesGroup {

        private class TestState : State {}
        public struct TestComponent : IComponent {}
        public struct TestComponent2 : IComponent {}

        private class TestDataConfigSystem : ISystem, IAdvanceTick {

            public World world { get; set; }

            public int testValueCount;
            private Filter filter;
            
            public void OnConstruct() {
                
                this.filter = Filter.Create("Test").With<TestComponent2>().Push();
                
            }

            public void OnDeconstruct() {
                
            }

            public void AdvanceTick(in float deltaTime) {
                
                NUnit.Framework.Assert.AreEqual(this.testValueCount, this.filter.Count);
                
            }

        }

        private class TestStatesHistoryModule : ME.ECS.StatesHistory.StatesHistoryModule<TestState> {

        }

        private class TestNetworkModule : ME.ECS.Network.NetworkModule<TestState> {

            protected override ME.ECS.Network.NetworkType GetNetworkType() {
                return ME.ECS.Network.NetworkType.RunLocal | ME.ECS.Network.NetworkType.SendToNet;
            }


        }

        [NUnit.Framework.TestAttribute]
        public void ApplyConfig() {

            var config = UnityEngine.Resources.Load<ME.ECS.DataConfigs.DataConfig>("Test");
            
            World world = null;
            WorldUtilities.CreateWorld<TestState>(ref world, 0.033f);
            {
                world.AddModule<TestStatesHistoryModule>();
                world.AddModule<TestNetworkModule>();
                world.SetState<TestState>(WorldUtilities.CreateState<TestState>());
                world.SetSeed(1u);
                {
                    WorldUtilities.InitComponentTypeId<TestComponent>(false);
                    WorldUtilities.InitComponentTypeId<TestComponent2>(false);
                    ComponentsInitializerWorld.Setup((e) => {
                
                        e.ValidateData<TestComponent>();
                        e.ValidateData<TestComponent2>();
                
                    });
                }
                {
                    
                    var count = 10000;
                    var group = new SystemGroup(world, "TestGroup");
                    group.AddSystem(new TestDataConfigSystem() {
                        testValueCount = count,
                    });
                    
                    config.Prewarm(true);

                    var entitiesGroup = world.AddEntities(count, Unity.Collections.Allocator.Temp, copyMode: true);
                    config.Apply(entitiesGroup);
                    entitiesGroup.Dispose();

                }
            }
            world.SaveResetState<TestState>();
            
            world.SetFromToTicks(0, 1);
            world.Update(1f);
            
            ComponentsInitializerWorld.Setup(null);
            WorldUtilities.ReleaseWorld<TestState>(ref world);

        }

    }

}
#endif