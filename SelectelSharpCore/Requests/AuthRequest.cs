using SelectelSharpCore.Headers;
using SelectelSharpCore.Responses;
using System.Net;
using System.Net.Http.Headers;

namespace SelectelSharpCore.Requests
{
    internal class AuthRequest : BaseRequest<AuthResponse>
    {
        /// <summary>
        /// Запрос токена для выполнения авторизованных запросов.
        /// </summary>
        /// <param name="user">Ваш номер договора (логин в общую панель управления http://support.selectel.ru/)</param>
        /// <param name="key">Пароль для данной услуги (он отличается от пароля для общей панели управления)</param>
        public AuthRequest(string user, string key)
        {
            TryAddHeader(HeaderKeys.XAuthUser, user);
            TryAddHeader(HeaderKeys.XAuthKey, key);
        }

        public override bool AllowAnonymously
        {
            get
            {
                return true;
            }
        }

        protected override string GetUrl(string storageUrl)
        {
            return "https://auth.selcdn.ru";
        }

        internal override void Parse(HttpResponseHeaders headers, object data, HttpStatusCode status)
        {
            this.Result = HeaderParsers.ParseHeaders<AuthResponse>(headers);
        }
    }
}
