using OpenSismApi.Models;
using DBContext.Models;
using DBContext.ViewModels;
using AutoMapper;
namespace OpenSismApi.Helpers
{
    public static class APIContants
    {
        public static string SUCCESS_RESULT = "success";
        public static string FAILED_RESULT = "failed";

        public static int SUCCESS_CODE = 1;

        public static int BAD_REQUEST_CODE = -1;

        public static int NOT_FOUND_CODE = -2;

        public static int SOMETHING_WENT_WROEG_CODE = -3;

        public static int INVALID_PASSWORD_CODE = -4;

        public static int INVALID_PHONE_CODE = -4;

        public static int INVALID_CODE_CODE = -4;

        public static int UNAUTHORIZED_CODE = -401;

        public static int VERSION_NOT_SUPPORTED_CODE = -333;

        public static int NEW_VERSION_CODE = 333;

        public static int ACCOUNT_NOT_VERIFIED_CODE = -444;

        public static int ACCOUNT_LOCKED_CODE = -555;

        public static int CONFIRMATION_EMAIL_TYPE = 1;
        public static int RESET_PASSWORD_EMAIL_TYPE = 2;

        public static int MISSING_HEADER_CODE = -5;
    }

    public static class APIContants<T>
    {
        public static string SUCCESS_RESULT = "success";
        public static string FAILED_RESULT = "failed";

        public static int SUCCESS_CODE = 1;

        public static int BAD_REQUEST_CODE = -1;

        public static int NOT_FOUND_CODE = -2;

        public static int SOMETHING_WENT_WROEG_CODE = -3;

        public static int INVALID_PASSWORD_CODE = -4;

        public static int INVALID_PHONE_CODE = -4;

        public static int INVALID_CODE_CODE = -4;

        public static int UNAUTHORIZED_CODE = -401;

        public static int VERSION_NOT_SUPPORTED_CODE = -333;

        public static int NEW_VERSION_CODE = 333;

        public static int ACCOUNT_NOT_VERIFIED_CODE = -444;

        public static int ACCOUNT_LOCKED_CODE = -555;

        public static int USER_EXIST_CODE = -777;
        public static int NO_POINTS_CODE = -888;

        public static int CONFIRMATION_EMAIL_TYPE = 1;
        public static int RESET_PASSWORD_EMAIL_TYPE = 2;


        public static Response<T> CostumSuccessResult(T Content, Customer customer  )
        {
            return new Response<T>(SUCCESS_CODE, SUCCESS_RESULT, Content, Mapper.Map<CustomerViewModel>(customer));
        }

        public static Response<T> CostumSuccessResult(T Content)
        {
            return new Response<T>(SUCCESS_CODE, SUCCESS_RESULT, Content, null);
        }

        public static Response<T> CostumBadResult(T t)
        {
            return CostumBadResult(null, t);
        }

        public static Response<T> CostumBadResult(string msg, T Content)
        {
            return new Response<T>(BAD_REQUEST_CODE, msg, Content, null);
        }

        public static Response<T> CostumNotFound(string msg, T Content)
        {
            return new Response<T>(NOT_FOUND_CODE, msg, Content, null);
        }

        public static Response<T> CostumSometingWrong(string msg, T Content)
        {
            return new Response<T>(SOMETHING_WENT_WROEG_CODE, msg, Content, null);
        }

        public static Response<T> CostumIncorectCode(string msg, T Content)
        {
            return new Response<T>(INVALID_CODE_CODE, msg, Content, null);
        }

        public static Response<T> CostumUserExist(string msg, T Content)
        {
            return new Response<T>(USER_EXIST_CODE, msg, Content, null);
        }

        public static Response<T> CostumNoPoints(string msg, T Content)
        {
            return new Response<T>(NO_POINTS_CODE, msg, Content, null);
        }
    }
}
