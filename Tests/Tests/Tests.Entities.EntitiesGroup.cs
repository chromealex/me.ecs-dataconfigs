#if !ENTITIES_GROUP_DISABLED
namespace ME.ECS.Tests {

    public class Tests_Entities_EntitiesGroup {

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
                    WorldUtilities.InitComponentTypeId<DataConfigTestComponent>(false);
                    ComponentsInitializerWorld.Setup((e) => {
                
                        e.ValidateData<TestComponent>();
                        e.ValidateData<DataConfigTestComponent>();
                
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
            
            WorldUtilities.ReleaseWorld<TestState>(ref world);

        }

    }

}
#endif