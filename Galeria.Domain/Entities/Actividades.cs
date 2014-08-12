using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Galeria.Domain;

namespace Galeria.Domain
{
   public class Actividades : IEntity
    {
        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }
        public virtual string Actividad { get; set; }
   
        public virtual DateTime hora { get; set; }
    }
}

