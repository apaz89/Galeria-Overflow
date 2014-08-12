using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Galeria.Domain
{
    public interface IEntity
    {
        long Id { get; set; }

        bool IsArchived { get; set; }
    }
}
