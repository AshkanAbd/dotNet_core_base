using dotNet_base.Components.Response;
using Microsoft.AspNetCore.Mvc;

namespace dotNet_base.Components.Extensions
{
    public interface IResponseExtension
    {
        public JsonResult Ok(object data = null, string msg = null)
        {
            return ResponseFormat.Ok(data, msg);
        }

        public JsonResult OkMsg(string msg = null)
        {
            return ResponseFormat.OkMsg(msg);
        }

        public JsonResult NotFound(object data = null, string msg = "داده پیدا نشد.")
        {
            return ResponseFormat.NotFound(data, msg);
        }

        public JsonResult NotFoundMsg(string msg = "داده پیدا نشد.")
        {
            return ResponseFormat.NotFoundMsg(msg);
        }

        public JsonResult PermissionDenied(object data = null, string msg = null)
        {
            return ResponseFormat.PermissionDenied(data, msg);
        }

        public JsonResult PermissionDeniedMsg(string msg = null)
        {
            return ResponseFormat.PermissionDeniedMsg(msg);
        }

        public JsonResult NotAuth(object data = null, string msg = "لطفا وارد سیستم شوید.")
        {
            return ResponseFormat.NotAuth(data, msg);
        }

        public JsonResult NotAuthMsg(string msg = "لطفا وارد سیستم شوید.")
        {
            return ResponseFormat.NotAuthMsg(msg);
        }

        public JsonResult BadRequest(object data = null, string msg = null)
        {
            return ResponseFormat.BadRequest(data, msg);
        }

        public JsonResult BadRequestMsg(string msg = null)
        {
            return ResponseFormat.BadRequestMsg(msg);
        }

        public JsonResult InternalError(object data = null, string msg = null)
        {
            return ResponseFormat.InternalError(data, msg);
        }

        public JsonResult InternalErrorMsg(string msg = null)
        {
            return ResponseFormat.InternalErrorMsg(msg);
        }
    }
}