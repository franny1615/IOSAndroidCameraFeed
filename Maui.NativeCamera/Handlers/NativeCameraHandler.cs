using Microsoft.Maui.Handlers;

namespace Maui.NativeCamera.Handlers;

public partial class NativeCameraHandler
{
    public static IPropertyMapper<NativeCameraView, NativeCameraHandler> PropertyMapper
        = new PropertyMapper<NativeCameraView, NativeCameraHandler>(ViewHandler.ViewMapper) {};

    public static CommandMapper<NativeCameraView, NativeCameraHandler> CommandMapper
        = new(ViewCommandMapper) {};

    public NativeCameraHandler() : base(PropertyMapper, CommandMapper) {}
}