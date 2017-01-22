using System;

namespace Cassiopeia.Models
{
    [Flags]
    internal enum Encryption
    {
        None = 0,
        PlainText = 1 << 0,
        Rc4Header = 1 << 1,
        Rc4Full = 1 << 2,
        All = PlainText | Rc4Full | Rc4Header
    }
}