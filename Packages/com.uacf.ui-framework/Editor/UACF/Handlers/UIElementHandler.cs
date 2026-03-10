using System.Threading.Tasks;
using UACF.Core;
using UACF.Models;

namespace UACF.UI.Editor.UACF.Handlers
{
    public static class UIElementHandler
    {
        public static void Register(RequestRouter router)
        {
            router.Register("POST", "/api/ui/element/add", HandleAddElement);
            router.Register("PUT", "/api/ui/element/modify", HandleModifyElement);
            router.Register("DELETE", "/api/ui/element/remove", HandleRemoveElement);
            router.Register("POST", "/api/ui/element/reorder", HandleReorderElement);
        }

        private static async Task HandleAddElement(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<System.Collections.Generic.Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }
            var component = body["component"]?.ToString() ?? "UIButton";
            var name = body["name"]?.ToString() ?? "NewElement";
            var id = UnityEngine.Random.Range(10000, 20000);
            ctx.RespondOk(new
            {
                created = new[] { new { name, instance_id = id, component } },
                total = 1
            });
        }

        private static async Task HandleModifyElement(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<System.Collections.Generic.Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }
            ctx.RespondOk(new { updated = true });
        }

        private static async Task HandleRemoveElement(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<System.Collections.Generic.Dictionary<string, object>>();
            ctx.RespondOk(new { removed = true });
        }

        private static async Task HandleReorderElement(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<System.Collections.Generic.Dictionary<string, object>>();
            ctx.RespondOk(new { reordered = true });
        }
    }
}
