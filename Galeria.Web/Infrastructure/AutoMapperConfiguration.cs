using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using Galeria.Domain;
using Galeria.Web.Controllers;
using Galeria.Web.Models;
using Ninject.Modules;

namespace Galeria.Web.Infrastructure
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.CreateMap<AccountSignUpModel, Account>();
            Mapper.CreateMap<Account, AccountSignUpModel>();
            
            Mapper.CreateMap<Account, AccountProfileModel>();
            Mapper.CreateMap<AccountProfileModel, Account>();
            
            Mapper.CreateMap<Account, RegisteredUsersListModel>();
            Mapper.CreateMap<RegisteredUsersListModel, Account>();
            
            Mapper.CreateMap<Account, ChangeUserSpaceLimitModel>();
            Mapper.CreateMap<ChangeUserSpaceLimitModel, Account>();

            Mapper.CreateMap<File,DiskContentModel>();
            Mapper.CreateMap<DiskContentModel, File>();
                      

            Mapper.CreateMap<File, FileSearchResult>();
            Mapper.CreateMap<FileSearchResult, File>();


            
            Mapper.CreateMap<File, FileSearchResult>();
            Mapper.CreateMap<FileSearchResult, File>();
                        

        }
    }
}