using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BootstrapMvcSample.Controllers;
using Galeria.Domain;
using Galeria.Domain.Services;
using Galeria.Web.Models;
using Galeria.Web.Utils;
using NHibernate.Properties;

namespace Galeria.Web.Controllers
{
    public class PackageListController : BootstrapBaseController
    {
        private readonly IReadOnlyRepository _readOnlyRepository;
        private readonly IWriteOnlyRepository _writeOnlyRepository;

        public PackageListController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
        }
    }
}
