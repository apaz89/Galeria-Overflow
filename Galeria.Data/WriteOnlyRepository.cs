using System;
using Galeria.Domain;
using Galeria.Domain.Services;
using NHibernate;

namespace Galeria.Data
{
    public class WriteOnlyRepository : IWriteOnlyRepository
    {
        private readonly ISession _session;

        public WriteOnlyRepository(ISession session)
        {
            _session = session;
        }

        public T Create<T>(T itemToCreate) where T : class, IEntity
        {
            _session.Save(itemToCreate);
            return itemToCreate;
        }

        public T Update<T>(T itemToUpdate) where T : class, IEntity
        {
            _session.Update(itemToUpdate);
            _session.Flush();
            return itemToUpdate;
        }

        public void Archive<T>(T itemToArchive)
        {
            throw new NotImplementedException();
        }

    }
}