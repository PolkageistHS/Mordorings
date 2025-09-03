using System.Drawing;

namespace Mordorings.Controls;

public interface IMapRendererBase
{
    Bitmap? GetMapSnapshot();
}
