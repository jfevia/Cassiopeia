using System.Collections;

namespace Cassiopeia.Windows.Controls
{
    public interface ITreeModel
    {
        IEnumerable GetChildren(object parent);

        bool HasChildren(object parent);
    }
}