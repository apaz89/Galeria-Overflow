﻿using System.Collections.Generic;



namespace Galeria.Domain
{
    public class Account : IEntity
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string LastName { get; set; }
        public virtual string EMail { get; set; }
        public virtual string Password { get; set; }
        public virtual bool IsArchived { get; set; }
        public virtual bool IsBlocked { get; set; }
        public virtual bool IsAdmin { get; set; }
        
        
        public virtual bool Isconfirmed { get; set; }


        public virtual IList<File> Files { get; set; }
        public virtual IList<Account> Referrals { get; set; }


    }
}