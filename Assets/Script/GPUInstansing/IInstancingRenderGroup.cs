using GPUInstancingRender;

public interface IInstancingRenderGroup
{
    void Register(IInstancingData data);
    void Unregister(IInstancingData data);
}
