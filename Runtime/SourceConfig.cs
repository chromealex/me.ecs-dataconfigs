namespace ME.ECS.DataConfigs {

    [ComponentGroup("Data Config", GroupColor.Default, -100)]
    [ComponentOrder(1)]
    [ComponentHelp("Stores last applied config")]
    public struct SourceConfig : IComponent, IComponentRuntime {

        public ConfigId<DataConfig> config;

    }

    #if !STATIC_API_DISABLED
    [ComponentGroup("Data Config", GroupColor.Default, -100)]
    [ComponentOrder(1)]
    [ComponentHelp("Stores all applied configs except first applied (see SourceConfig component)")]
    public struct SourceConfigs : IComponent, IComponentRuntime, IComponentDisposable<SourceConfigs> {

        public ME.ECS.Collections.LowLevel.List<ConfigId<DataConfig>> configs;

        public void OnDispose(ref ME.ECS.Collections.LowLevel.Unsafe.MemoryAllocator allocator) {
            if (this.configs.isCreated == true) this.configs.Dispose(ref allocator);
        }

        public void ReplaceWith(ref ME.ECS.Collections.LowLevel.Unsafe.MemoryAllocator allocator, in SourceConfigs other) {
            this.configs.ReplaceWith(ref allocator, other.configs);
        }
        
        public void CopyFrom(ref ME.ECS.Collections.LowLevel.Unsafe.MemoryAllocator allocator, in SourceConfigs other) {
            this.configs.CopyFrom(ref allocator, other.configs);
        }

    }
    #endif

}