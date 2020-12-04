using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using dotNet_base.Models;
using dotNet_base.Components;
using dotNet_base.Components.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace dotNet_base.Components.Extensions
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/v1")]
    public class ControllerExtension : Controller, IPaginationExtension, IResponseExtension, IUploadExtension
    {
        protected ComponentConfig ComponentConfig;
        public BaseContext Context;
        public IWebHostEnvironment Environment { get; }
        public object AuthenticatedUser { get; set; }
        public List<object> DataBag { get; set; } = new List<object>();
        protected IServiceScopeFactory ServiceScopeFactory { get; }

        public ControllerExtension(BaseContext context, IWebHostEnvironment environment,
            IOptions<ComponentConfig> config, IServiceScopeFactory serviceScopeFactory)
        {
            ComponentConfig = config.Value;
            Context = context;
            Environment = environment;
            ServiceScopeFactory = serviceScopeFactory;
        }

        protected string GenerateCode(int length = 5, bool useLetter = false)
        {
            var random = new Random((int) DateTime.Now.ToFileTime());
            string data;
            if (useLetter) {
                data = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            }
            else {
                data = "0123456789";
            }

            var chars = Enumerable.Range(0, length)
                .Select(x => data[random.Next(0, data.Length)]);
            return new string(chars.ToArray());
        }

        protected JsonResult Ok(object data = null, string msg = null)
        {
            return (this as IResponseExtension).Ok(data, msg);
        }

        protected JsonResult OkMsg(string msg = null)
        {
            return (this as IResponseExtension).OkMsg(msg);
        }

        protected JsonResult NotFound(object data = null, string msg = "داده پیدا نشد.")
        {
            return (this as IResponseExtension).NotFound(data, msg);
        }

        protected JsonResult NotFoundMsg(string msg = "داده پیدا نشد.")
        {
            return (this as IResponseExtension).NotFoundMsg(msg);
        }

        protected JsonResult PermissionDenied(object data = null, string msg = null)
        {
            return (this as IResponseExtension).PermissionDenied(data, msg);
        }

        protected JsonResult PermissionDeniedMsg(string msg = null)
        {
            return (this as IResponseExtension).PermissionDeniedMsg(msg);
        }

        protected JsonResult NotAuth(object data = null, string msg = "لطفا وارد سیستم شوید.")
        {
            return (this as IResponseExtension).NotAuth(data, msg);
        }

        protected JsonResult NotAuthMsg(string msg = "لطفا وارد سیستم شوید.")
        {
            return (this as IResponseExtension).NotAuthMsg(msg);
        }

        protected JsonResult BadRequest(object data = null, string msg = null)
        {
            return (this as IResponseExtension).BadRequest(data, msg);
        }

        protected JsonResult BadRequestMsg(string msg = null)
        {
            return (this as IResponseExtension).BadRequestMsg(msg);
        }

        protected JsonResult InternalError(object data = null, string msg = null)
        {
            return (this as IResponseExtension).InternalError(data, msg);
        }

        protected JsonResult InternalErrorMsg(string msg = null)
        {
            return (this as IResponseExtension).InternalErrorMsg(msg);
        }
    }
}