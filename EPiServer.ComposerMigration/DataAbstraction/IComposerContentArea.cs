using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPiServer.ComposerMigration.DataAbstraction
{
    /// <summary>
    /// Represents a Composer Page or (Layout) Block
    /// </summary>
    public interface IComposerContentArea
    {
        string Name { get; }

        List<IComposerContent> Blocks { get; }
    }
}
