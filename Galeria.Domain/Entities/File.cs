using System;
using System.Collections.Generic;

namespace Galeria.Domain
{
    public class File : IEntity
    {
        public virtual long? FileShared_id { get; set; }
        public virtual long Id { get; set; }
        public virtual string Url { get; set; }
        public virtual string Name { get; set; }
        public virtual string Type { get; set; }
        public virtual int FileSize { get; set; }
        public virtual byte[] Image{ get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual bool IsArchived { get; set; }
        public virtual bool IsDirectory { get; set; }
    }
}