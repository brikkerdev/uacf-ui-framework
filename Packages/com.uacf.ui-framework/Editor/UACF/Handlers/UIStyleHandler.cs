using System.Threading.Tasks;
using UACF.Core;
using UACF.Models;

namespace UACF.UI.Editor.UACF.Handlers
{
    public static class UIStyleHandler
    {
        public static void Register(RequestRouter router)
        {
            router.Register("POST", "/api/ui/style/create", HandleCreateStyle);
            router.Register("PUT", "/api/ui/style/apply", HandleApplyStyle);
            router.Register("GET", "/api/ui/style/list", HandleListStyles);
        }

        private static async Task HandleCreateStyle(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<System.Collections.Generic.Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }
            ctx.RespondOk(new { key = body["key"]?.ToString() ?? "new_style", path = "" });
        }

        private static async Task HandleApplyStyle(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<System.Collections.Generic.Dictionary<string, object>>();
            ctx.RespondOk(new { applied = true });
        }

        private static async Task HandleListStyles(RequestContext ctx)
        {
            var data = await MainThreadDispatcher.Enqueue(() =>
            {
                return new
                {
                    styles = new object[]
                    {
                        new { key = "button_filled", path = "", has_parent = false },
                        new { key = "button_outlined", path = "", has_parent = false }
                    }
                };
            });
            ctx.RespondOk(data);
        }
    }
}
