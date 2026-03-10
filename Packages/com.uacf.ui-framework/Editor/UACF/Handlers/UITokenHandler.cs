using System.Threading.Tasks;
using UACF.Core;
using UACF.Models;

namespace UACF.UI.Editor.UACF.Handlers
{
    public static class UITokenHandler
    {
        public static void Register(RequestRouter router)
        {
            router.Register("PUT", "/api/ui/tokens/colors", HandleUpdateColors);
            router.Register("POST", "/api/ui/tokens/colors/add-custom", HandleAddCustomColor);
            router.Register("PUT", "/api/ui/tokens/typography", HandleUpdateTypography);
            router.Register("PUT", "/api/ui/tokens/spacing", HandleUpdateSpacing);
        }

        private static async Task HandleUpdateColors(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<System.Collections.Generic.Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }
            ctx.RespondOk(new { updated = new string[0], theme_reapplied = true });
        }

        private static async Task HandleAddCustomColor(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<System.Collections.Generic.Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }
            ctx.RespondOk(new { added = true });
        }

        private static async Task HandleUpdateTypography(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<System.Collections.Generic.Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }
            ctx.RespondOk(new { updated = true });
        }

        private static async Task HandleUpdateSpacing(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<System.Collections.Generic.Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }
            ctx.RespondOk(new { updated = true });
        }
    }
}
