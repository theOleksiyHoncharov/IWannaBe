using GPUInstancingRender;
using System.Collections.Generic;

public class InstancedRenderGroupManager
{
    private readonly Dictionary<GPUInstancingGroupType, IInstancingRenderGroup> _groups =
        new Dictionary<GPUInstancingGroupType, IInstancingRenderGroup>();

    public void RegisterGroup(GPUInstancingGroupType groupType, IInstancingRenderGroup group)
    {
        _groups[groupType] = group;
    }

    public IInstancingRenderGroup GetGroup(GPUInstancingGroupType groupType)
    {
        IInstancingRenderGroup group;
        _groups.TryGetValue(groupType, out group);
        return group;
    }
}
