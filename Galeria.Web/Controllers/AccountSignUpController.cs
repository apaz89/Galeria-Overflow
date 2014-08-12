using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using Amazon.S3.Model;
using AutoMapper;
using BootstrapMvcSample.Controllers;
using Galeria.Domain;
using Galeria.Domain.Services;
using Galeria.Web.Models;
using Galeria.Web.Utils;
using File = System.IO.File;

namespace Galeria.Web.Controllers
{
    public class AccountSignUpController : BootstrapBaseController
    {
        private readonly IReadOnlyRepository _readOnlyRepository;
        private readonly IWriteOnlyRepository _writeOnlyRepository;
        

        public AccountSignUpController( IWriteOnlyRepository writeOnlyRepository, IReadOnlyRepository readOnlyRepository)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
        }
        
        [HttpGet]
        public ActionResult AccountSignUp(long token)
        {
            Session["userReferralId"] = token;
            
            return View(new AccountSignUpModel());
        }
       
        public ActionResult Cancelar()
        {
            return RedirectToAction("LogIn","Account");
        }

        [HttpPost]
        public ActionResult AccountSignUp(AccountSignUpModel model)
        {
            var result = _readOnlyRepository.Query<Account>(a=>a.EMail==model.EMail);

            if (result.Any())
            {
                Error("Email account is already registered in this site!!!");
                return View(model);
            }

            var account = Mapper.Map<Account>(model);
            account.IsArchived = false;
            account.IsAdmin = false;
            account.IsBlocked = false;
            account.Password = EncriptacionMD5.Encriptar(model.Password);
            account.Isconfirmed = false;


           var createdAccount= _writeOnlyRepository.Create(account);

            var token = Convert.ToInt64(Session["userReferralId"]);

            if (token != 0)
            {
                var userReferring = _readOnlyRepository.GetById<Account>(token);
                userReferring.Referrals.Add(createdAccount);
                _writeOnlyRepository.Update(userReferring);
            }

            var serverFolderPath = Server.MapPath("~/App_Data/UploadedFiles/" + account.EMail);
            Directory.CreateDirectory(serverFolderPath);
                              
            
            // ESTOOOOOOO
            #region EnvioCorreoParaNotificacion

            var fechaActual = DateTime.Now.Date;

            var pass = result.FirstOrDefault().Id;
            var data = "" + fechaActual.Day + fechaActual.Month + fechaActual.Year;
            var tokenConfir = pass + ";" + EncriptacionMD5.Encriptar(data);

            //var url = "http://Galeria-1.apphb.com/PasswordReset/PasswordReset";
            var url = "http://Galeriaclase.apphb.com/Account/Confirmed";

            var emailBody = new StringBuilder("<b>Confirm your account of Galeria</b>");
            emailBody.Append("<br/>");
            emailBody.Append("<br/>");
            emailBody.Append("<b>" + url + "?token=" + tokenConfir + "<b>");
            emailBody.Append("<br/>");
            emailBody.Append("<br/>");
            emailBody.Append("<b>This link is only valid through " + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "</b>");

            if (MailSender.SendEmail(model.EMail, "Confirm your account of Galeria", emailBody.ToString()))
                return Cancelar();

            Error("E-Mail failed to be sent, please try again!!!");
            return View(model);
            #endregion


            return Cancelar();
        }
              

    }
}
