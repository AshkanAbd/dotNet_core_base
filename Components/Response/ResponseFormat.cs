using Microsoft.AspNetCore.Mvc;

namespace dotNet_base.Components.Response
{
    public class ResponseFormat
    {
        public static JsonResult Base(string status = null, string msg = null, object data = null,
            int statusCode = 200)
        {
            return new JsonResult(new StdResponse(status, msg, data)) {
                StatusCode = statusCode
            };
        }

        public static JsonResult BaseError(string msg = null, object data = null, int statusCode = 400)
        {
            return Base("error", msg, data, statusCode);
        }

        public static JsonResult BaseValidationError(string msg = null, object data = null)
        {
            return Base("validation-error", msg, data, 422);
        }

        public static JsonResult BaseSuccess(string msg = null, object data = null)
        {
            return Base("success", msg, data);
        }

        public static JsonResult Ok(object data = null, string msg = null)
        {
            return BaseSuccess(msg, data);
        }

        public static JsonResult OkMsg(string msg = null)
        {
            return BaseSuccess(msg);
        }

        public static JsonResult NotFound(object data = null, string msg = "داده پیدا نشد.")
        {
            return BaseError(msg, data, 404);
        }

        public static JsonResult NotFoundMsg(string msg = "داده پیدا نشد.")
        {
            return BaseError(msg, null, 404);
        }

        public static JsonResult PermissionDenied(object data = null, string msg = null)
        {
            return BaseError(msg, data, 403);
        }

        public static JsonResult PermissionDeniedMsg(string msg = null)
        {
            return BaseError(msg, null, 403);
        }

        public static JsonResult NotAuth(object data = null, string msg = "لطفا وارد سیستم شوید.")
        {
            return BaseError(msg, data, 401);
        }

        public static JsonResult NotAuthMsg(string msg = "لطفا وارد سیستم شوید.")
        {
            return BaseError(msg, null, 401);
        }

        public static JsonResult BadRequest(object data = null, string msg = null)
        {
            return BaseValidationError(msg, data);
        }

        public static JsonResult BadRequestMsg(string msg = null)
        {
            return BaseValidationError(msg);
        }

        public static JsonResult InternalError(object data = null, string msg = null)
        {
            return BaseError(msg, data, 500);
        }

        public static JsonResult InternalErrorMsg(string msg = null)
        {
            return BaseError(msg, null, 500);
        }
    }
}